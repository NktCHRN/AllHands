using AllHands.Shared.Domain.Abstractions;

namespace AllHands.EmployeeService.Domain.Events.Employee;

public sealed record EmployeeUpdatedEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    Guid PositionId,
    Guid ManagerId,
    string Email, 
    string NormalizedEmail,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    DateOnly WorkStartDate,
    Guid GlobalUserId,
    Guid RoleId): AuditableEvent(EntityId, PerformedByUserId);
    