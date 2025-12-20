namespace AllHands.Shared.Contracts.Messaging.Events;

public interface IAllHandsEvent
{
    string? GroupId { get; }
    DateTimeOffset OccurredAt { get; }
}
