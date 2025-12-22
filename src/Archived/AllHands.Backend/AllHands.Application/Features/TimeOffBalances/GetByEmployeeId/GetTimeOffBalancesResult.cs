namespace AllHands.Application.Features.TimeOffBalances.GetByEmployeeId;

public sealed record GetTimeOffBalancesResult(IReadOnlyList<TimeOffBalanceDto> Balances);
