using Microsoft.AspNetCore.Mvc;
using MtTicket.API.DTOs.Common;
using MtTicket.API.DTOs.User;
using MtTicket.API.Services;
using System.Net;

namespace MtTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly ILoginRateLimiter _loginRateLimiter;

    public AuthController(IAuthService authService, ILogger<AuthController> logger, ILoginRateLimiter loginRateLimiter)
    {
        _authService = authService;
        _logger = logger;
        _loginRateLimiter = loginRateLimiter;
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
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (_loginRateLimiter.IsLimitReached($"login:{clientIp}", 5, TimeSpan.FromMinutes(1)))
            {
                return StatusCode((int)HttpStatusCode.TooManyRequests,
                    ApiResponse<AuthResult>.ErrorResponse("Vượt quá số lần đăng nhập, thử lại sau 1 phút"));
            }

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

    /// <summary>
    /// Refresh token để lấy access token mới
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResult>>> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                return BadRequest(ApiResponse<AuthResult>.ErrorResponse("Refresh token không hợp lệ"));
            }

            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            if (!result.Success)
            {
                return Unauthorized(ApiResponse<AuthResult>.ErrorResponse(result.ErrorMessage ?? "Refresh token không hợp lệ"));
            }

            return Ok(ApiResponse<AuthResult>.SuccessResponse(result, "Làm mới token thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi refresh token");
            return StatusCode(500, ApiResponse<AuthResult>.ErrorResponse("Lỗi server"));
        }
    }
}
