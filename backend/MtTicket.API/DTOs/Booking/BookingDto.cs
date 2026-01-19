namespace MtTicket.API.DTOs.Booking;

/// <summary>
/// DTO trả về thông tin booking
/// </summary>
public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<BookingItemDetailDto> Items { get; set; } = new();
}

/// <summary>
/// DTO cho chi tiết booking item
/// </summary>
public class BookingItemDetailDto
{
    public int TicketId { get; set; }
    public string TicketType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}