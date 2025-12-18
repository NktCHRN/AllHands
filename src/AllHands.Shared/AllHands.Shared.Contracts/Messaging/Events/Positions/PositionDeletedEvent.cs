namespace AllHands.Shared.Contracts.Messaging.Events.Positions;

public sealed record PositionDeletedEvent(Guid Id, Guid CompanyId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
