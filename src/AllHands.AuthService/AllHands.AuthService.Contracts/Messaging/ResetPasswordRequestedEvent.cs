using AllHands.Shared.Contracts.Messaging.Events;

namespace AllHands.Auth.Contracts.Messaging;

public record ResetPasswordRequestedEvent(string Email, string FirstName, string Token) : IAllHandsEvent
{
    public string GroupId => null!;
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
