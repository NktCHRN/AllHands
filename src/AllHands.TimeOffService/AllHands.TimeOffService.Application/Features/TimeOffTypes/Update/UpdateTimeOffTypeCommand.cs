using AllHands.TimeOffService.Domain.Models;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.Update;

public record UpdateTimeOffTypeCommand(
    int Order,
    string Name,
    string Emoji,
    decimal DaysPerYear,
    TimeOffPerYearUpdateType? TimeOffUpdateType) : TimeOffTypeBaseCommand(Name, Emoji, DaysPerYear), IRequest
{
    public Guid Id { get; set; }
}
