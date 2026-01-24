using Microsoft.EntityFrameworkCore;
using MtTicket.API.Data;
using MtTicket.API.DTOs.Common;
using MtTicket.API.Models;
using MtTicket.API.Repositories.Common;

namespace MtTicket.API.Repositories.Booking;

/// <summary>
/// Repository implementation cho Booking
/// </summary>
public class BookingRepository : Repository<Models.Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResponse<Models.Booking>> GetUserBookingsAsync(int userId, PaginationParams paginationParams)
    {
        var query = _dbSet
            .Where(b => b.UserId == userId)
            .Include(b => b.User) // để map UserName không bị null
            .Include(b => b.Event)
            .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Ticket)
            .AsQueryable();

        // Sắp xếp
        query = paginationParams.SortDescending
            ? query.OrderByDescending(b => b.CreatedAt)
            : query.OrderBy(b => b.CreatedAt);

        var totalCount = await query.CountAsync();

        var bookings = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return new PagedResponse<Models.Booking>
        {
            Data = bookings,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Models.Booking?> GetBookingWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.User)
            .Include(b => b.Event)
            .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Ticket)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Models.Booking>> GetBookingsByStatusAsync(string status)
    {
        return await _dbSet
            .Where(b => b.Status == status)
            .Include(b => b.User)
            .Include(b => b.Event)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Booking>> GetBookingsByEventAsync(int eventId)
    {
        return await _dbSet
            .Where(b => b.EventId == eventId)
            .Include(b => b.User)
            .Include(b => b.BookingItems)
            .ToListAsync();
    }
}
