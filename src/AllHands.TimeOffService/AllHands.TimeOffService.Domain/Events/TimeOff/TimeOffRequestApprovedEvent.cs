using AllHands.Shared.Domain.Abstractions;

namespace AllHands.TimeOffService.Domain.Events.TimeOff;

public sealed record TimeOffRequestApprovedEvent(
    Guid EntityId, 
    Guid PerformedByUserId, 
    Guid PerformedByEmployeeId) : AuditableEvent(EntityId, PerformedByUserId);
