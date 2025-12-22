using AllHands.Shared.Domain.Abstractions;

namespace AllHands.EmployeeService.Domain.Events.Employee;

public sealed record EmployeeReactivatedEvent(Guid EntityId, Guid PerformedByUserId, Guid GlobalUserId, Guid RoleId): AuditableEvent(EntityId, PerformedByUserId);  // Fired employee could be rehired.
