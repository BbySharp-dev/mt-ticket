using Microsoft.EntityFrameworkCore;

namespace MtTicket.API.Data;

/// <summary>
/// DbContext chính của ứng dụng
/// Quản lý kết nối và cấu hình database
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}