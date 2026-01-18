using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtTicket.API.Models;

/// <summary>
/// Model đại diện cho loại vé
/// </summary>
[Table("Tickets")]
public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TicketType { get; set; } = string.Empty; // Normal, VIP, EarlyBird

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public int Available { get; set; }

    // Foreign key relationship
    [ForeignKey("EventId")]
    public virtual Event Event { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
}