namespace AllHands.Shared.Contracts.Messaging.Events.Employees;

public sealed record EmployeeCreatedEvent(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email, 
    string? PhoneNumber,
    DateOnly WorkStartDate,
    Guid ManagerId,
    Guid PositionId,
    Guid CompanyId,
    Guid UserId,
    string Status) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
    