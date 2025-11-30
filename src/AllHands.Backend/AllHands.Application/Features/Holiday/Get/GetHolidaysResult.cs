using AllHands.Application.Dto;

namespace AllHands.Application.Features.Holiday.Get;

public sealed record GetHolidaysResult(IReadOnlyList<HolidayDto> Holidays);