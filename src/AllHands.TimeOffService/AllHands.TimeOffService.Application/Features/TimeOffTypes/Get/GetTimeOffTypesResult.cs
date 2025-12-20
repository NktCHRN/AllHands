namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.Get;

public sealed record GetTimeOffTypesResult(IReadOnlyList<TimeOffTypeDto> TimeOffTypes);
