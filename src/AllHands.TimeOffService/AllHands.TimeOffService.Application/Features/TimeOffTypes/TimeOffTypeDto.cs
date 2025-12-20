using AllHands.TimeOffService.Domain.Models;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes;

public sealed record TimeOffTypeDto(
    Guid Id,
    int Order,
    string Name,
    string Emoji,
    decimal DaysPerYear)
{
    public static TimeOffTypeDto FromModel(TimeOffType model)
    {
        return new TimeOffTypeDto(model.Id, model.Order, model.Name, model.Emoji, model.DaysPerYear);
    }
}
    