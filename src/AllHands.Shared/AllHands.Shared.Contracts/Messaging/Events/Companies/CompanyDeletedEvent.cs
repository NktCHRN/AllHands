namespace AllHands.Shared.Contracts.Messaging.Events.Companies;

public sealed record CompanyDeletedEvent(
    Guid Id) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
