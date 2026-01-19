namespace MtTicket.API.DTOs.Booking;

/// <summary>
/// DTO để tạo booking mới
/// </summary>
public class CreateBookingDto
{
    public int EventId { get; set; }
    public List<BookingItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO cho từng loại vé trong booking
/// </summary>
public class BookingItemDto
{
    public int TicketId { get; set; }
    public int Quantity { get; set; }
}