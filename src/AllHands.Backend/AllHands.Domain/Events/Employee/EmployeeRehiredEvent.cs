using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public sealed record EmployeeRehiredEvent(Guid EntityId, Guid PerformedByUserId): AuditableEvent(EntityId, PerformedByUserId);  // Fired employee could be rehired.
