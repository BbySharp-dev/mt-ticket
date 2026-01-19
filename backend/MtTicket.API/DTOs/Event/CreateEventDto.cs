namespace MtTicket.API.DTOs.Event;

/// <summary>
/// DTO để tạo event mới
/// Validation sẽ được thực hiện bằng FluentValidation
/// </summary>
public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime EventDate { get; set; } // Start Date
    public DateTime? EndDate { get; set; } // Optional - End Date
    public string Location { get; set; } = string.Empty;
    public int TotalTickets { get; set; }
    public decimal Price { get; set; }
}