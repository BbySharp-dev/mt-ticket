using MtTicket.API.DTOs.Common;
using MtTicket.API.DTOs.Event;

namespace MtTicket.API.Services;

/// <summary>
/// Service interface cho Event
/// </summary>
public interface IEventService
{
    Task<PagedResponse<EventDto>> GetPagedEventsAsync(PaginationParams paginationParams);
    Task<EventDto?> GetEventByIdAsync(int id);
    Task<EventDto> CreateEventAsync(CreateEventDto createEventDto);
    Task<EventDto?> UpdateEventAsync(int id, UpdateEventDto updateEventDto);
    Task<bool> DeleteEventAsync(int id);
    Task<IEnumerable<EventDto>> GetUpcomingEventsAsync(int count = 10);
}