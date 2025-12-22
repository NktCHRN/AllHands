using AllHands.Shared.Domain.Abstractions;

namespace AllHands.EmployeeService.Domain.Events.Employee;

public sealed record EmployeeRegisteredEvent(
    Guid EntityId, 
    Guid PerformedByUserId): AuditableEvent(EntityId, PerformedByUserId);