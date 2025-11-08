using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public sealed record EmployeeDeletedEvent(Guid EntityId, Guid PerformedByUserId): AuditableEvent(EntityId, PerformedByUserId);