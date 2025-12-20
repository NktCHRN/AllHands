namespace AllHands.Auth.Contracts.Messaging;

public sealed record UserSessionsRecalculationRequestedEvent(Guid UserId, Guid RequesterId)
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
