using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalanceRequestChangeEvent(
    Guid EntityId, 
    Guid PerformedByUserId, 
    Guid TimeOffRequestId,
    decimal Delta) : AuditableEvent(EntityId, PerformedByUserId);
