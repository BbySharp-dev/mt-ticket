using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MtTicket.API.DTOs.Common;
using MtTicket.API.DTOs.Event;
using MtTicket.API.Services;

namespace MtTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventService eventService, ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<EventDto>>>> GetEvents([FromQuery] PaginationParams paginationParams)
    {
        try
        {
            var events = await _eventService.GetPagedEventsAsync(paginationParams);
            return Ok(ApiResponse<PagedResponse<EventDto>>.SuccessResponse(events));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách events");
            return StatusCode(500, ApiResponse<PagedResponse<EventDto>>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<EventDto>>> GetEvent(int id)
    {
        try
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);

            if (eventDto == null)
            {
                return NotFound(ApiResponse<EventDto>.ErrorResponse("Event không tồn tại"));
            }

            return Ok(ApiResponse<EventDto>.SuccessResponse(eventDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy event {EventId}", id);
            return StatusCode(500, ApiResponse<EventDto>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<ApiResponse<IEnumerable<EventDto>>>> GetUpcomingEvents([FromQuery] int count = 10)
    {
        try
        {
            var events = await _eventService.GetUpcomingEventsAsync(count);
            return Ok(ApiResponse<IEnumerable<EventDto>>.SuccessResponse(events.ToList()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy upcoming events");
            return StatusCode(500, ApiResponse<IEnumerable<EventDto>>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<EventDto>>> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<EventDto>.ErrorResponse("Dữ liệu không hợp lệ", errors));
            }

            var eventDto = await _eventService.CreateEventAsync(createEventDto);

            return CreatedAtAction(nameof(GetEvent), new { id = eventDto.Id },
                ApiResponse<EventDto>.SuccessResponse(eventDto, "Tạo event thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<EventDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo event");
            return StatusCode(500, ApiResponse<EventDto>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<EventDto>>> UpdateEvent(int id, [FromBody] UpdateEventDto updateEventDto)
    {
        try
        {
            var eventDto = await _eventService.UpdateEventAsync(id, updateEventDto);

            if (eventDto == null)
            {
                return NotFound(ApiResponse<EventDto>.ErrorResponse("Event không tồn tại"));
            }

            return Ok(ApiResponse<EventDto>.SuccessResponse(eventDto, "Cập nhật event thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<EventDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật event {EventId}", id);
            return StatusCode(500, ApiResponse<EventDto>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEvent(int id)
    {
        try
        {
            var result = await _eventService.DeleteEventAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Event không tồn tại"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Xóa event thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa event {EventId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Lỗi server"));
        }
    }
}
