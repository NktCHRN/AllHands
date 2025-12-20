using AllHands.Shared.Contracts.Messaging.Events;

namespace AllHands.Auth.Contracts.Messaging;

public sealed record UserInvitedEvent(
    string Email,
    string FirstName,
    string AdminName,
    Guid InvitationId,
    string Token) : IAllHandsEvent
{
    public string GroupId => null!;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
    