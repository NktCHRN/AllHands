namespace AllHands.Shared.Contracts.Messaging.Events.Employees;

public record EmployeeStatusUpdated(
    Guid Id,
    string Status,
    Guid CompanyId,
    Guid UserId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
