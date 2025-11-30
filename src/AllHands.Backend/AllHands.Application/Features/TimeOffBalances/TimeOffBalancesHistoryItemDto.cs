using AllHands.Domain.Models;

namespace AllHands.Application.Features.TimeOffBalances;

public sealed record TimeOffBalancesHistoryItemDto(
    Guid BalanceId,
    Guid TypeId,
    string TypeName,
    string TypeEmoji,
    Employee? UpdatedBy,
    DateTimeOffset Timestamp,
    decimal Delta);
