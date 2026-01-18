using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtTicket.API.Models;

/// <summary>
/// Model đại diện cho chi tiết từng loại vé trong đơn đặt
/// </summary>
[Table("BookingItems")]
public class BookingItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BookingId { get; set; }

    [Required]
    public int TicketId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    // Foreign key relationships
    [ForeignKey("BookingId")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("TicketId")]
    public virtual Ticket Ticket { get; set; } = null!;
}