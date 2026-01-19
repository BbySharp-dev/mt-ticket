namespace MtTicket.API.DTOs.User;

/// <summary>
/// DTO cho đăng nhập
/// Validation sẽ được thực hiện bằng FluentValidation
/// </summary>
public class LoginDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}