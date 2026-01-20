using Microsoft.EntityFrameworkCore;
using MtTicket.API.Data;
using MtTicket.API.DTOs.Common;
using MtTicket.API.Models;
using MtTicket.API.Repositories.Common;

namespace MtTicket.API.Repositories.Event;

/// <summary>
/// Repository implementation cho Event
/// </summary>
public class EventRepository : Repository<Models.Event>, IEventRepository
{
    public EventRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResponse<Models.Event>> GetPagedEventsAsync(PaginationParams paginationParams)
    {
        var query = _dbSet.AsQueryable();

        // Tìm kiếm
        if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
        {
            query = query.Where(e => 
                e.Title.Contains(paginationParams.SearchTerm) ||
                e.Description != null && e.Description.Contains(paginationParams.SearchTerm) ||
                e.Location.Contains(paginationParams.SearchTerm));
        }

        // Sắp xếp
        query = paginationParams.SortBy?.ToLower() switch
        {
            "title" => paginationParams.SortDescending 
                ? query.OrderByDescending(e => e.Title)
                : query.OrderBy(e => e.Title),
            "date" => paginationParams.SortDescending
                ? query.OrderByDescending(e => e.EventDate)
                : query.OrderBy(e => e.EventDate),
            "price" => paginationParams.SortDescending
                ? query.OrderByDescending(e => e.Price)
                : query.OrderBy(e => e.Price),
            _ => query.OrderBy(e => e.EventDate) // Mặc định sắp xếp theo ngày
        };

        // Đếm tổng số
        var totalCount = await query.CountAsync();

        // Phân trang
        var events = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return new PagedResponse<Models.Event>
        {
            Data = events,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IEnumerable<Models.Event>> GetUpcomingEventsAsync(int count = 10)
    {
        return await _dbSet
            .Where(e => e.EventDate >= DateTime.UtcNow && e.AvailableTickets > 0)
            .OrderBy(e => e.EventDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(e => e.EventDate >= startDate && e.EventDate <= endDate)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<Models.Event?> GetEventWithTicketsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> UpdateAvailableTicketsAsync(int eventId, int quantity)
    {
        var eventEntity = await _dbSet.FindAsync(eventId);
        if (eventEntity == null)
            return false;

        eventEntity.AvailableTickets += quantity;
        eventEntity.UpdatedAt = DateTime.UtcNow;
        
        return true;
    }
}
