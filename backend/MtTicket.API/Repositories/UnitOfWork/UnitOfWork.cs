using Microsoft.EntityFrameworkCore.Storage;
using MtTicket.API.Data;
using MtTicket.API.Repositories.Booking;
using MtTicket.API.Repositories.Common;
using MtTicket.API.Repositories.Event;
using MtTicket.API.Repositories.User;

namespace MtTicket.API.Repositories.UnitOfWork;

/// <summary>
/// Unit of Work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy initialization cho repositories
    private IUserRepository? _users;
    private IEventRepository? _events;
    private IBookingRepository? _bookings;
    private IRepository<Models.Ticket>? _tickets;
    private IRepository<Models.BookingItem>? _bookingItems;
    // Các repositories cho tính năng nâng cao 
    // private IRepository<Models.Venue>? _venues;
    // private IRepository<Models.Seat>? _seats;
    // private IRepository<Models.TicketReservation>? _ticketReservations;
    // private IRepository<Models.Payment>? _payments;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ApplicationDbContext Context => _context;

    public IUserRepository Users => 
        _users ??= new UserRepository(_context);

    public IEventRepository Events => 
        _events ??= new EventRepository(_context);

    public IBookingRepository Bookings => 
        _bookings ??= new BookingRepository(_context);

    public IRepository<Models.Ticket> Tickets => 
        _tickets ??= new Repository<Models.Ticket>(_context);

    public IRepository<Models.BookingItem> BookingItems => 
        _bookingItems ??= new Repository<Models.BookingItem>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
