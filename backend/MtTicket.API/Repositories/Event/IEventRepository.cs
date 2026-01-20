using MtTicket.API.DTOs.Common;
using MtTicket.API.Models;
using MtTicket.API.Repositories.Common;

namespace MtTicket.API.Repositories.Event;

/// <summary>
/// Repository interface cho Event
/// Thêm các methods đặc biệt cho Event
/// </summary>
public interface IEventRepository : IRepository<Models.Event>
{
    // Lấy events với phân trang và tìm kiếm
    Task<PagedResponse<Models.Event>> GetPagedEventsAsync(PaginationParams paginationParams);
    
    // Lấy events sắp diễn ra
    Task<IEnumerable<Models.Event>> GetUpcomingEventsAsync(int count = 10);
    
    // Lấy events theo ngày
    Task<IEnumerable<Models.Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    // Lấy event với tickets
    Task<Models.Event?> GetEventWithTicketsAsync(int id);
    
    // Cập nhật số vé còn lại
    Task<bool> UpdateAvailableTicketsAsync(int eventId, int quantity);
}
