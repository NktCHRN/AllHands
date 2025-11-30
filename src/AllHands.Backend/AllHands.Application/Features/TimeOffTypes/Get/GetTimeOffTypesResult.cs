namespace AllHands.Application.Features.TimeOffTypes.Get;

public sealed record GetTimeOffTypesResult(IReadOnlyList<TimeOffTypeDto> TimeOffTypes);
