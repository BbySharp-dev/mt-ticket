using AutoMapper;
using MtTicket.API.DTOs.Booking;
using MtTicket.API.DTOs.Common;
using MtTicket.API.Models;
using MtTicket.API.Repositories.UnitOfWork;

namespace MtTicket.API.Services;

/// <summary>
/// Service implementation cho Booking
/// </summary>
public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BookingDto> CreateBookingAsync(int userId, CreateBookingDto createBookingDto)
    {
        // Bắt đầu transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Kiểm tra event tồn tại
            var eventEntity = await _unitOfWork.Events.GetEventWithTicketsAsync(createBookingDto.EventId);
            if (eventEntity == null)
            {
                throw new InvalidOperationException("Event không tồn tại");
            }

            // Kiểm tra event chưa diễn ra
            if (eventEntity.EventDate < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Event đã diễn ra");
            }

            // Kiểm tra số vé còn lại
            if (eventEntity.AvailableTickets <= 0)
            {
                throw new InvalidOperationException("Hết vé");
            }

            // Tính tổng tiền và validate tickets
            decimal totalAmount = 0;
            var bookingItems = new List<BookingItem>();

            foreach (var itemDto in createBookingDto.Items)
            {
                var ticket = eventEntity.Tickets.FirstOrDefault(t => t.Id == itemDto.TicketId);
                
                if (ticket == null)
                {
                    throw new InvalidOperationException($"Ticket ID {itemDto.TicketId} không tồn tại");
                }

                if (ticket.Available < itemDto.Quantity)
                {
                    throw new InvalidOperationException($"Không đủ vé loại {ticket.TicketType}");
                }

                totalAmount += ticket.Price * itemDto.Quantity;

                // Tạo booking item
                bookingItems.Add(new BookingItem
                {
                    TicketId = ticket.Id,
                    Quantity = itemDto.Quantity,
                    Price = ticket.Price
                });

                // Cập nhật số vé còn lại
                ticket.Available -= itemDto.Quantity;
                _unitOfWork.Tickets.Update(ticket);
            }

            // Kiểm tra tổng số vé không vượt quá available
            var totalQuantity = createBookingDto.Items.Sum(i => i.Quantity);
            if (totalQuantity > eventEntity.AvailableTickets)
            {
                throw new InvalidOperationException("Số vé yêu cầu vượt quá số vé còn lại");
            }

            // Tạo booking
            var booking = new Booking
            {
                UserId = userId,
                EventId = createBookingDto.EventId,
                BookingDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync(); // Lưu để có Booking.Id

            // Thêm booking items
            foreach (var item in bookingItems)
            {
                item.BookingId = booking.Id;
            }
            await _unitOfWork.BookingItems.AddRangeAsync(bookingItems);

            // Cập nhật số vé còn lại của event
            eventEntity.AvailableTickets -= totalQuantity;
            _unitOfWork.Events.Update(eventEntity);

            // Commit transaction
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Load lại với đầy đủ thông tin
            var bookingWithDetails = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(booking.Id);
            return _mapper.Map<BookingDto>(bookingWithDetails);
        }
        catch
        {
            // Rollback nếu có lỗi
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<PagedResponse<BookingDto>> GetUserBookingsAsync(int userId, PaginationParams paginationParams)
    {
        var pagedBookings = await _unitOfWork.Bookings.GetUserBookingsAsync(userId, paginationParams);
        
        return new PagedResponse<BookingDto>
        {
            Data = _mapper.Map<List<BookingDto>>(pagedBookings.Data),
            PageNumber = pagedBookings.PageNumber,
            PageSize = pagedBookings.PageSize,
            TotalCount = pagedBookings.TotalCount
        };
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int id)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(id);
        
        if (booking == null)
            return null;

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<bool> CancelBookingAsync(int bookingId, int userId)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
        
        if (booking == null)
            return false;

        // Kiểm tra quyền (chỉ user sở hữu mới được cancel)
        if (booking.UserId != userId)
        {
            throw new UnauthorizedAccessException("Không có quyền hủy booking này");
        }

        // Kiểm tra đã cancel chưa
        if (booking.Status == "Cancelled")
        {
            throw new InvalidOperationException("Booking đã được hủy");
        }

        // Bắt đầu transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Hoàn lại số vé
            foreach (var item in booking.BookingItems)
            {
                var ticket = await _unitOfWork.Tickets.GetByIdAsync(item.TicketId);
                if (ticket != null)
                {
                    ticket.Available += item.Quantity;
                    _unitOfWork.Tickets.Update(ticket);
                }
            }

            // Cập nhật số vé của event
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(booking.EventId);
            if (eventEntity != null)
            {
                var totalQuantity = booking.BookingItems.Sum(i => i.Quantity);
                eventEntity.AvailableTickets += totalQuantity;
                _unitOfWork.Events.Update(eventEntity);
            }

            // Cập nhật status
            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Bookings.Update(booking);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}