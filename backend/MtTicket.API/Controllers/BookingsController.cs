using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MtTicket.API.DTOs.Booking;
using MtTicket.API.DTOs.Common;
using MtTicket.API.Helpers;
using MtTicket.API.Services;

namespace MtTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CreateBooking([FromBody] CreateBookingDto createBookingDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<BookingDto>.ErrorResponse("Dữ liệu không hợp lệ", errors));
            }

            var userId = UserHelper.GetUserId(User);
            var bookingDto = await _bookingService.CreateBookingAsync(userId, createBookingDto);

            return CreatedAtAction(nameof(GetBooking), new { id = bookingDto.Id },
                ApiResponse<BookingDto>.SuccessResponse(bookingDto, "Đặt vé thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BookingDto>.ErrorResponse(ex.Message));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(ApiResponse<BookingDto>.ErrorResponse("Chưa đăng nhập hoặc token không hợp lệ"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo booking");
            return StatusCode(500, ApiResponse<BookingDto>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpGet("my-bookings")]
    public async Task<ActionResult<ApiResponse<PagedResponse<BookingDto>>>> GetMyBookings([FromQuery] PaginationParams paginationParams)
    {
        try
        {
            var userId = UserHelper.GetUserId(User);
            var bookings = await _bookingService.GetUserBookingsAsync(userId, paginationParams);
            return Ok(ApiResponse<PagedResponse<BookingDto>>.SuccessResponse(bookings));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(ApiResponse<PagedResponse<BookingDto>>.ErrorResponse("Chưa đăng nhập hoặc token không hợp lệ"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách bookings");
            return StatusCode(500, ApiResponse<PagedResponse<BookingDto>>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetBooking(int id)
    {
        try
        {
            var bookingDto = await _bookingService.GetBookingByIdAsync(id);

            if (bookingDto == null)
            {
                return NotFound(ApiResponse<BookingDto>.ErrorResponse("Booking không tồn tại"));
            }

            var userId = UserHelper.GetUserId(User);
            if (bookingDto.UserId != userId)
            {
                return Forbid();
            }

            return Ok(ApiResponse<BookingDto>.SuccessResponse(bookingDto));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(ApiResponse<BookingDto>.ErrorResponse("Chưa đăng nhập hoặc token không hợp lệ"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy booking {BookingId}", id);
            return StatusCode(500, ApiResponse<BookingDto>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ApiResponse<bool>>> CancelBooking(int id)
    {
        try
        {
            var userId = UserHelper.GetUserId(User);
            var result = await _bookingService.CancelBookingAsync(id, userId);

            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Booking không tồn tại"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Hủy booking thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(ApiResponse<bool>.ErrorResponse("Chưa đăng nhập hoặc token không hợp lệ"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi hủy booking {BookingId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Lỗi server"));
        }
    }
}
