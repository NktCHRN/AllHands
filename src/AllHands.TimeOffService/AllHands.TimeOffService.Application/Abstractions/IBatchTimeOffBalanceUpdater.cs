using AllHands.TimeOffService.Application.Features.TimeOffBalances.UpdateAll;

namespace AllHands.TimeOffService.Application.Abstractions;

public interface IBatchTimeOffBalanceUpdater
{
    Task<TimeOffBalancesUpdateResult> UpdateAllAsync(CancellationToken cancellationToken);
}