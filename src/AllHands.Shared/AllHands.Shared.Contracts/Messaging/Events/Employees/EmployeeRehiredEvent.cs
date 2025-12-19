namespace AllHands.Shared.Contracts.Messaging.Events.Employees;

public sealed record EmployeeRehiredEvent(
    Guid Id,
    Guid CompanyId,
    Guid UserId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
