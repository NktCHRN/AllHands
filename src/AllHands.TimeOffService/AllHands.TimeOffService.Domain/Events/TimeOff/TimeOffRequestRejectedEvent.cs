using AllHands.Shared.Domain.Abstractions;

namespace AllHands.TimeOffService.Domain.Events.TimeOff;

public sealed record TimeOffRequestRejectedEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    Guid PerformedByEmployeeId,
    string Reason) : AuditableEvent(EntityId, PerformedByUserId);
    