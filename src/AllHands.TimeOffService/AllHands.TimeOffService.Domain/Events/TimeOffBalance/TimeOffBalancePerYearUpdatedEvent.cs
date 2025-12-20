using AllHands.Shared.Domain.Abstractions;
using AllHands.TimeOffService.Domain.Models;

namespace AllHands.TimeOffService.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalancePerYearUpdatedEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    decimal? Amount,
    decimal? Delta,
    TimeOffPerYearUpdateType UpdateType) : AuditableEvent(EntityId, PerformedByUserId);
    