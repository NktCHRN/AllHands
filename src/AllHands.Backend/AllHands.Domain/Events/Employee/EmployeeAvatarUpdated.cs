using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public sealed record EmployeeAvatarUpdated(
    Guid EntityId, 
    Guid PerformedByUserId): AuditableEvent(EntityId, PerformedByUserId);
