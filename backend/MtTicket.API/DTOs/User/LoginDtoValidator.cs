using FluentValidation;

namespace MtTicket.API.DTOs.User;

/// <summary>
/// Validator cho LoginDto
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty().WithMessage("Username hoặc Email là bắt buộc");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password là bắt buộc");
    }
}