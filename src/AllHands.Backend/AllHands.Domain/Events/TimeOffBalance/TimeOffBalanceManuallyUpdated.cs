using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalanceManuallyUpdated(
    Guid EntityId, 
    Guid PerformedByUserId, 
    Guid PerformedByEmployeeId,
    string Reason,
    decimal Amount) : AuditableEvent(EntityId, PerformedByUserId);
