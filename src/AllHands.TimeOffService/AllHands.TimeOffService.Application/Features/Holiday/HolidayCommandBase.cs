namespace AllHands.TimeOffService.Application.Features.Holiday;

public abstract record HolidayCommandBase(string Name, DateOnly Date);
