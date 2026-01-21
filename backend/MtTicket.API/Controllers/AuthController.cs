using Microsoft.AspNetCore.Mvc;
using MtTicket.API.DTOs.Common;
using MtTicket.API.DTOs.User;
using MtTicket.API.Services;

namespace MtTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResult>>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<AuthResult>.ErrorResponse("Dữ liệu không hợp lệ", errors));
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(ApiResponse<AuthResult>.ErrorResponse(result.ErrorMessage ?? "Đăng ký thất bại"));
            }

            return Ok(ApiResponse<AuthResult>.SuccessResponse(result, "Đăng ký thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng ký user");
            return StatusCode(500, ApiResponse<AuthResult>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResult>>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<AuthResult>.ErrorResponse("Dữ liệu không hợp lệ", errors));
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(ApiResponse<AuthResult>.ErrorResponse(result.ErrorMessage ?? "Đăng nhập thất bại"));
            }

            return Ok(ApiResponse<AuthResult>.SuccessResponse(result, "Đăng nhập thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng nhập");
            return StatusCode(500, ApiResponse<AuthResult>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpPost("validate-token")]
    public async Task<ActionResult<ApiResponse<bool>>> ValidateToken([FromBody] string token)
    {
        try
        {
            var isValid = await _authService.ValidateTokenAsync(token);
            return Ok(ApiResponse<bool>.SuccessResponse(isValid, isValid ? "Token hợp lệ" : "Token không hợp lệ"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi validate token");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Lỗi server"));
        }
    }
}
