using AllHands.Shared.Domain.Abstractions;

namespace AllHands.EmployeeService.Domain.Events.Employee;

public sealed record EmployeeRehiredEvent(Guid EntityId, Guid PerformedByUserId): AuditableEvent(EntityId, PerformedByUserId);  // Fired employee could be rehired.
