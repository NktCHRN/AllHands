namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.UpdateAll;

public sealed record TimeOffBalancesUpdateResult(long RowsProcessed, long RowsUpdated, bool HasFailures);
