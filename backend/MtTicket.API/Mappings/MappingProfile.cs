using AutoMapper;
using MtTicket.API.DTOs.Booking;
using MtTicket.API.DTOs.Event;
using MtTicket.API.DTOs.User;
using MtTicket.API.Models;

namespace MtTicket.API.Mappings;

/// <summary>
/// Cấu hình AutoMapper - map giữa Model và DTO
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Sẽ hash sau
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Event mappings
        CreateMap<Event, EventDto>();
        CreateMap<CreateEventDto, Event>()
            .ForMember(dest => dest.AvailableTickets, opt => opt.MapFrom(src => src.TotalTickets))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateEventDto, Event>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Chỉ update field không null

        // Booking mappings
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title));

        CreateMap<BookingItem, BookingItemDetailDto>()
            .ForMember(dest => dest.TicketType, opt => opt.MapFrom(src => src.Ticket.TicketType));
    }
}