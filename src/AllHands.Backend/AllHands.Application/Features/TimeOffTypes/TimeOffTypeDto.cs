namespace AllHands.Application.Features.TimeOffTypes;

public sealed record TimeOffTypeDto(
    Guid Id,
    int Order,
    string Name,
    string Emoji,
    decimal DaysPerYear);
    