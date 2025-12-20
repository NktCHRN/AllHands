using AllHands.Shared.Domain.Abstractions;

namespace AllHands.TimeOffService.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalanceRequestChangeEvent(
    Guid EntityId, 
    Guid PerformedByUserId, 
    Guid TimeOffRequestId,
    decimal Delta) : AuditableEvent(EntityId, PerformedByUserId);
