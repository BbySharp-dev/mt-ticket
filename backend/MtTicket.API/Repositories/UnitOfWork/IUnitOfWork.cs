using MtTicket.API.Data;
using MtTicket.API.Models;
using MtTicket.API.Repositories.Booking;
using MtTicket.API.Repositories.Common;
using MtTicket.API.Repositories.Event;
using MtTicket.API.Repositories.User;

namespace MtTicket.API.Repositories.UnitOfWork;

/// <summary>
/// Unit of Work interface
/// Quản lý transaction và repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IEventRepository Events { get; }
    IBookingRepository Bookings { get; }
    IRepository<Ticket> Tickets { get; }
    IRepository<BookingItem> BookingItems { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    
    ApplicationDbContext Context { get; } // Expose DbContext để dùng trong advanced features

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
