using AutoMapper;
using MtTicket.API.DTOs.Common;
using MtTicket.API.DTOs.Event;
using MtTicket.API.Repositories.Event;
using MtTicket.API.Repositories.UnitOfWork;

namespace MtTicket.API.Services;

/// <summary>
/// Service implementation cho Event
/// </summary>
public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<EventDto>> GetPagedEventsAsync(PaginationParams paginationParams)
    {
        var pagedEvents = await _unitOfWork.Events.GetPagedEventsAsync(paginationParams);
        
        return new PagedResponse<EventDto>
        {
            Data = _mapper.Map<List<EventDto>>(pagedEvents.Data),
            PageNumber = pagedEvents.PageNumber,
            PageSize = pagedEvents.PageSize,
            TotalCount = pagedEvents.TotalCount
        };
    }

    public async Task<EventDto?> GetEventByIdAsync(int id)
    {
        var eventEntity = await _unitOfWork.Events.GetEventWithTicketsAsync(id);
        
        if (eventEntity == null)
            return null;

        return _mapper.Map<EventDto>(eventEntity);
    }

    public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDto)
    {
        // Validate business rules
        if (createEventDto.EventDate < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Ngày sự kiện không thể ở quá khứ");
        }

        if (createEventDto.TotalTickets <= 0)
        {
            throw new InvalidOperationException("Số vé phải lớn hơn 0");
        }

        var eventEntity = _mapper.Map<Models.Event>(createEventDto);
        await _unitOfWork.Events.AddAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventDto>(eventEntity);
    }

    public async Task<EventDto?> UpdateEventAsync(int id, UpdateEventDto updateEventDto)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
        
        if (eventEntity == null)
            return null;

        // Validate nếu có cập nhật ngày
        if (updateEventDto.EventDate.HasValue && updateEventDto.EventDate < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Ngày sự kiện không thể ở quá khứ");
        }

        // Map các field được update
        _mapper.Map(updateEventDto, eventEntity);
        eventEntity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Events.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventDto>(eventEntity);
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
        
        if (eventEntity == null)
            return false;

        // Kiểm tra xem có bookings nào chưa
        // bookings nằm ở booking repository, không phải event repository
        var bookings = await _unitOfWork.Bookings.GetBookingsByEventAsync(id);
        if (bookings.Any())
        {
            throw new InvalidOperationException("Không thể xóa event đã có bookings");
        }

        _unitOfWork.Events.Remove(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync(int count = 10)
    {
        var events = await _unitOfWork.Events.GetUpcomingEventsAsync(count);
        return _mapper.Map<List<EventDto>>(events);
    }
}