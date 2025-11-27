using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.TimeOff;

public sealed record TimeOffRequestRejectedEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    Guid PerformedByEmployeeId,
    string Reason,
    decimal WorkingDaysCount,
    Guid TimeOffBalanceId) : AuditableEvent(EntityId, PerformedByUserId);
    