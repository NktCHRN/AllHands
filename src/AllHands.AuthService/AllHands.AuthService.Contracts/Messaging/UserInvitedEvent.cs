namespace AllHands.Auth.Contracts.Messaging;

public sealed record UserInvitedEvent(
    string Email,
    string FirstName,
    string AdminName,
    Guid InvitationId,
    string Token);
    