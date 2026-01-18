using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtTicket.API.Models;

/// <summary>
/// Model đại diện cho đơn đặt vé
/// </summary>
[Table("Bookings")]
public class Booking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    public DateTime BookingDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key relationships
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("EventId")]
    public virtual Event Event { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
}