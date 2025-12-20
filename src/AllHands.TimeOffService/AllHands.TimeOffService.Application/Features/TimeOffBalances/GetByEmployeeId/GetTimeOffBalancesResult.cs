namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.GetByEmployeeId;

public sealed record GetTimeOffBalancesResult(IReadOnlyList<TimeOffBalanceDto> Balances);
