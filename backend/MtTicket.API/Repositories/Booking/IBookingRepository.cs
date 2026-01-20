using MtTicket.API.DTOs.Common;
using MtTicket.API.Models;
using MtTicket.API.Repositories.Common;

namespace MtTicket.API.Repositories.Booking;

/// <summary>
/// Repository interface cho Booking
/// </summary>
public interface IBookingRepository : IRepository<Models.Booking>
{
    // Lấy bookings của user với phân trang
    Task<PagedResponse<Models.Booking>> GetUserBookingsAsync(int userId, PaginationParams paginationParams);
    
    // Lấy booking với đầy đủ thông tin
    Task<Models.Booking?> GetBookingWithDetailsAsync(int id);
    
    // Lấy bookings theo status
    Task<IEnumerable<Models.Booking>> GetBookingsByStatusAsync(string status);
    
    // Lấy bookings theo event
    Task<IEnumerable<Models.Booking>> GetBookingsByEventAsync(int eventId);
}
