using FluentValidation;

namespace MtTicket.API.DTOs.Booking;

/// <summary>
/// Validator cho CreateBookingDto
/// </summary>
public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.EventId)
            .GreaterThan(0).WithMessage("Event ID không hợp lệ");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Danh sách vé là bắt buộc")
            .Must(items => items.Count > 0).WithMessage("Phải chọn ít nhất 1 vé");

        RuleForEach(x => x.Items)
            .SetValidator(new BookingItemDtoValidator());
    }
}

/// <summary>
/// Validator cho BookingItemDto
/// </summary>
public class BookingItemDtoValidator : AbstractValidator<BookingItemDto>
{
    public BookingItemDtoValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("Ticket ID không hợp lệ");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0");
    }
}