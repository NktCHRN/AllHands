using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

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
    DateOnly WorkStartDate): AuditableEvent(EntityId, PerformedByUserId);
    