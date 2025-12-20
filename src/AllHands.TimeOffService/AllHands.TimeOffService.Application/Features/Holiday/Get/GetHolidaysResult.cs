using AllHands.TimeOffService.Application.Dto;

namespace AllHands.TimeOffService.Application.Features.Holiday.Get;

public sealed record GetHolidaysResult(IReadOnlyList<HolidayDto> Holidays);