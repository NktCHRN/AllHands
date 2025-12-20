namespace AllHands.TimeOffService.Application.Features.TimeOffTypes;

public abstract record TimeOffTypeBaseCommand(
    string Name,
    string Emoji,
    decimal DaysPerYear);
