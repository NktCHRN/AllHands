using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public sealed record EmployeeRegisteredEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    string? AvatarFileName): AuditableEvent(EntityId, PerformedByUserId);