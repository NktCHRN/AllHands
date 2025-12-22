using AllHands.Shared.Domain.Abstractions;

namespace AllHands.EmployeeService.Domain.Events.Employee;

public sealed record EmployeeFiredEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    string Reason): AuditableEvent(EntityId, PerformedByUserId);
    