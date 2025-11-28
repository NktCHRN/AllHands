using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public sealed record EmployeeAvatarUpdatedEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    string? AvatarFileName): AuditableEvent(EntityId, PerformedByUserId);
