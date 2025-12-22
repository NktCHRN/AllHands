using AllHands.Shared.Domain.Abstractions;

namespace AllHands.EmployeeService.Domain.Events.Employee;

public sealed record EmployeeDeletedEvent(Guid EntityId, Guid PerformedByUserId, string Reason): AuditableEvent(EntityId, PerformedByUserId);