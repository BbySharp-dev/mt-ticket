using FluentValidation;

namespace MtTicket.API.DTOs.Event;

/// <summary>
/// Validator cho UpdateEventDto
/// </summary>
public class UpdateEventDtoValidator : AbstractValidator<UpdateEventDto>
{
    public UpdateEventDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Tiêu đề không được quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Mô tả không được quá 2000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("URL hình ảnh không được quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Địa điểm không được quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.TotalTickets)
            .GreaterThan(0).WithMessage("Tổng số vé phải lớn hơn 0")
            .When(x => x.TotalTickets.HasValue);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá vé phải lớn hơn 0")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.EndDate)
            .Must((dto, endDate) => !endDate.HasValue || !dto.EventDate.HasValue || endDate.Value > dto.EventDate.Value)
            .WithMessage("Ngày kết thúc phải sau ngày bắt đầu")
            .When(x => x.EndDate.HasValue && x.EventDate.HasValue);
    }
}