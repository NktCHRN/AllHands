namespace AllHands.TimeOffService.Application.Dto;

public sealed record HolidayDto(Guid Id, string Name, DateOnly Date);
