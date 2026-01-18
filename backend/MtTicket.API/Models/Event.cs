using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtTicket.API.Models;

/// <summary>
/// Model đại diện cho sự kiện
/// </summary>
[Table("Events")]
public class Event
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public DateTime EventDate { get; set; } // Start Date

    public DateTime? EndDate { get; set; } // Optional - End Date (sẽ thêm validation sau)

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required]
    public int TotalTickets { get; set; }

    [Required]
    public int AvailableTickets { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}