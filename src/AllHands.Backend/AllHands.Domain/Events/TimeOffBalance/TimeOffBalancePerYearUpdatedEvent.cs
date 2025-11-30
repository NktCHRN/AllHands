using AllHands.Domain.Abstractions;
using AllHands.Domain.Models;

namespace AllHands.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalancePerYearUpdatedEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    decimal OldDaysPerYear,
    decimal NewDaysPerYear,
    TimeOffPerYearUpdateType UpdateType) : AuditableEvent(EntityId, PerformedByUserId);
    