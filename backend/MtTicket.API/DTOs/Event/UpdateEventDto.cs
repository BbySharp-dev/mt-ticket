namespace MtTicket.API.DTOs.Event;

/// <summary>
/// DTO để cập nhật event
/// Validation sẽ được thực hiện bằng FluentValidation
/// </summary>
public class UpdateEventDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? EventDate { get; set; } // Start Date
    public DateTime? EndDate { get; set; } // End Date
    public string? Location { get; set; }
    public int? TotalTickets { get; set; }
    public decimal? Price { get; set; }
}