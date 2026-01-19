using FluentValidation;

namespace MtTicket.API.DTOs.Event;

/// <summary>
/// Validator cho CreateEventDto
/// </summary>
public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
{
    public CreateEventDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề là bắt buộc")
            .MaximumLength(200).WithMessage("Tiêu đề không được quá 200 ký tự");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Mô tả không được quá 2000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("URL hình ảnh không được quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.EventDate)
            .NotEmpty().WithMessage("Ngày bắt đầu sự kiện là bắt buộc")
            .Must(date => date > DateTime.UtcNow).WithMessage("Ngày bắt đầu phải sau thời điểm hiện tại");

        RuleFor(x => x.EndDate)
            .Must((dto, endDate) => !endDate.HasValue || endDate.Value > dto.EventDate)
            .WithMessage("Ngày kết thúc phải sau ngày bắt đầu")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Địa điểm là bắt buộc")
            .MaximumLength(200).WithMessage("Địa điểm không được quá 200 ký tự");

        RuleFor(x => x.TotalTickets)
            .GreaterThan(0).WithMessage("Tổng số vé phải lớn hơn 0");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá vé phải lớn hơn 0");
    }
}