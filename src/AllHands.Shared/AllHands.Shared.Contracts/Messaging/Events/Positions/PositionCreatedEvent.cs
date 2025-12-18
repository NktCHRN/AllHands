namespace AllHands.Shared.Contracts.Messaging.Events.Positions;

public sealed record PositionCreatedEvent(Guid Id, string Name, Guid CompanyId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
