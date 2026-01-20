using MtTicket.API.DTOs.Booking;
using MtTicket.API.DTOs.Common;

namespace MtTicket.API.Services;

/// <summary>
/// Service interface cho Booking
/// </summary>
public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(int userId, CreateBookingDto createBookingDto);
    Task<PagedResponse<BookingDto>> GetUserBookingsAsync(int userId, PaginationParams paginationParams);
    Task<BookingDto?> GetBookingByIdAsync(int id);
    Task<bool> CancelBookingAsync(int bookingId, int userId);
}