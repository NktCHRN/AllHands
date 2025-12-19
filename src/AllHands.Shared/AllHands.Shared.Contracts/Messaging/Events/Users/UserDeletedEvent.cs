namespace AllHands.Shared.Contracts.Messaging.Events.Users;

public sealed record UserDeletedEvent(Guid Id, Guid CompanyId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
