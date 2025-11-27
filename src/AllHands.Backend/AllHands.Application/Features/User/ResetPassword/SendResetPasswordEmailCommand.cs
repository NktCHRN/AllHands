namespace AllHands.Application.Features.User.ResetPassword;

public sealed record SendResetPasswordEmailCommand(string Email, string FirstName, string Token);
