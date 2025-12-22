using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.TimeOff;

public sealed record TimeOffRequestCancelledEvent(
    Guid EntityId, 
    Guid PerformedByUserId) : AuditableEvent(EntityId, PerformedByUserId);
    