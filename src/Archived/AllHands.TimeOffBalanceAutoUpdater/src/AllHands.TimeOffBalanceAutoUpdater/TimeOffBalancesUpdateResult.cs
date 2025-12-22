namespace AllHands.TimeOffBalanceAutoUpdater;

public sealed record TimeOffBalancesUpdateResult(long RowsProcessed, long RowsUpdated, bool HasFailures);
