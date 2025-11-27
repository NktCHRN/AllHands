using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public sealed record EmployeeCreatedEvent(
    Guid EntityId, 
    Guid PerformedByUserId, 
    Guid UserId,
    Guid CompanyId,
    Guid PositionId,
    Guid ManagerId,
    string Email, 
    string NormalizedEmail,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    DateOnly WorkStartDate): AuditableEvent(EntityId, PerformedByUserId);
