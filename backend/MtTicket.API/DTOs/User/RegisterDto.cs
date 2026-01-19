namespace MtTicket.API.DTOs.User;

/// <summary>
/// DTO cho đăng ký user mới
/// Validation sẽ được thực hiện bằng FluentValidation
/// </summary>
public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
}