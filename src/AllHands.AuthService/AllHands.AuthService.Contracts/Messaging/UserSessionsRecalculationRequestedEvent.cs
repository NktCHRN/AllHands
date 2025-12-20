using AllHands.Shared.Contracts.Messaging.Events;

namespace AllHands.Auth.Contracts.Messaging;

public sealed record UserSessionsRecalculationRequestedEvent(Guid UserId, Guid RequesterId) : IAllHandsEvent
{
    public string GroupId => UserId.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
