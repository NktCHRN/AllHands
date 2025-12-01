using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.TimeOff;

public sealed record TimeOffRequestApprovedEvent(
    Guid EntityId, 
    Guid PerformedByUserId, 
    Guid PerformedByEmployeeId) : AuditableEvent(EntityId, PerformedByUserId);
