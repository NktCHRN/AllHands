using AllHands.Shared.Domain.Abstractions;

namespace AllHands.EmployeeService.Domain.Events.Employee;

public sealed record EmployeeUpdatedBySelfEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber): AuditableEvent(EntityId, PerformedByUserId);
