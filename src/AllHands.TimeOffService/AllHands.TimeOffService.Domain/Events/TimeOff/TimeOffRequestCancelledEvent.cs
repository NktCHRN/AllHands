using AllHands.Shared.Domain.Abstractions;

namespace AllHands.TimeOffService.Domain.Events.TimeOff;

public sealed record TimeOffRequestCancelledEvent(
    Guid EntityId, 
    Guid PerformedByUserId) : AuditableEvent(EntityId, PerformedByUserId);
    