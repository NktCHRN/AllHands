using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public sealed record EmployeeInfoUpdatedEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    Guid PositionId,
    Guid ManagerId,
    string Email, 
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    DateOnly WorkStartDate,
    string? AvatarFileName): AuditableEvent(EntityId, PerformedByUserId);
    