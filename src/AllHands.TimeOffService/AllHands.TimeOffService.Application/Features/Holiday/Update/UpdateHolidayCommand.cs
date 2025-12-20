using MediatR;

namespace AllHands.TimeOffService.Application.Features.Holiday.Update;

public sealed record UpdateHolidayCommand(string Name, DateOnly Date) : HolidayCommandBase(Name, Date), IRequest
{
    public Guid Id { get; set; }    
}
