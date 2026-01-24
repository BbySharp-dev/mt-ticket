using Microsoft.EntityFrameworkCore;
using MtTicket.API.Models;

namespace MtTicket.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets - đại diện cho các bảng trong database
    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingItem> BookingItems { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Cấu hình Event
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasIndex(e => e.EventDate);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        // Cấu hình Ticket
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasOne(t => t.Event)
                  .WithMany(e => e.Tickets)
                  .HasForeignKey(t => t.EventId)
                  .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Event nếu còn Ticket
        });

        // Cấu hình Booking
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasOne(b => b.User)
                  .WithMany(u => u.Bookings)
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.Event)
                  .WithMany(e => e.Bookings)
                  .HasForeignKey(b => b.EventId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.BookingDate);
            entity.HasIndex(e => e.Status);
        });

        // Cấu hình BookingItem
        modelBuilder.Entity<BookingItem>(entity =>
        {
            entity.HasOne(bi => bi.Booking)
                  .WithMany(b => b.BookingItems)
                  .HasForeignKey(bi => bi.BookingId)
                  .OnDelete(DeleteBehavior.Cascade); // Xóa Booking thì xóa luôn BookingItems

            entity.HasOne(bi => bi.Ticket)
                  .WithMany(t => t.BookingItems)
                  .HasForeignKey(bi => bi.TicketId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Cấu hình RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(rt => rt.Token).IsUnique();

            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}