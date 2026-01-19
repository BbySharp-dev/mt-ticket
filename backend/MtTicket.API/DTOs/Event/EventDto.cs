namespace MtTicket.API.DTOs.Event;

/// <summary>
/// DTO trả về thông tin event
/// </summary>
public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime EventDate { get; set; } // Start Date
    public DateTime? EndDate { get; set; } // End Date (Optional)
    public string Location { get; set; } = string.Empty;
    public int TotalTickets { get; set; }
    public int AvailableTickets { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}