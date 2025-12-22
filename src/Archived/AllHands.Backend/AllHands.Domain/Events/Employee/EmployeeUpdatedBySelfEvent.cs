using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public sealed record EmployeeUpdatedBySelfEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber): AuditableEvent(EntityId, PerformedByUserId);
