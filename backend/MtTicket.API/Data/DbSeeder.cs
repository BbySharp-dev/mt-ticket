using Microsoft.EntityFrameworkCore;
using MtTicket.API.Models;

namespace MtTicket.API.Data;

/// <summary>
/// Class để seed dữ liệu mẫu vào database
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Kiểm tra xem đã có dữ liệu chưa
        if (await context.Events.AnyAsync())
        {
            return; // Đã có dữ liệu rồi, không seed nữa
        }

        // Tạo users mẫu
        var users = new List<User>
        {
            new User
            {
                Username = "admin",
                Email = "admin@mtticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // Sẽ cài BCrypt sau
                FullName = "Administrator",
                PhoneNumber = "0123456789"
            },
            new User
            {
                Username = "user1",
                Email = "user1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                FullName = "Nguyễn Văn A",
                PhoneNumber = "0987654321"
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Tạo events mẫu
        var events = new List<Event>
        {
            new Event
            {
                Title = "Concert Nhạc Pop 2024",
                Description = "Buổi concert nhạc pop đặc sắc với nhiều ca sĩ nổi tiếng",
                ImageUrl = "https://via.placeholder.com/400x300",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Nhà hát lớn Hà Nội",
                TotalTickets = 1000,
                AvailableTickets = 1000,
                Price = 500000
            },
            new Event
            {
                Title = "Lễ hội Ẩm thực Việt Nam",
                Description = "Khám phá nền ẩm thực đa dạng của Việt Nam",
                ImageUrl = "https://via.placeholder.com/400x300",
                EventDate = DateTime.UtcNow.AddDays(45),
                Location = "Công viên Thống Nhất",
                TotalTickets = 500,
                AvailableTickets = 500,
                Price = 200000
            }
        };

        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();

        // Tạo tickets mẫu cho mỗi event
        var tickets = new List<Ticket>();
        foreach (var evt in events)
        {
            tickets.Add(new Ticket
            {
                EventId = evt.Id,
                TicketType = "Normal",
                Price = evt.Price,
                Quantity = evt.TotalTickets / 2,
                Available = evt.TotalTickets / 2
            });

            tickets.Add(new Ticket
            {
                EventId = evt.Id,
                TicketType = "VIP",
                Price = evt.Price * 2,
                Quantity = evt.TotalTickets / 2,
                Available = evt.TotalTickets / 2
            });
        }

        await context.Tickets.AddRangeAsync(tickets);
        await context.SaveChangesAsync();
    }
}