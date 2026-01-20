using MtTicket.API.DTOs.User;

namespace MtTicket.API.Services;

/// <summary>
/// Service interface cho authentication
/// </summary>
public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterDto registerDto);
    Task<AuthResult> LoginAsync(LoginDto loginDto);
    Task<bool> ValidateTokenAsync(string token);
}

/// <summary>
/// Kết quả authentication
/// </summary>
public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }
}