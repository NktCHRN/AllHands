namespace AllHands.Auth.Contracts.Messaging;

public record ResetPasswordRequestedEvent(string Email, string FirstName, string Token);