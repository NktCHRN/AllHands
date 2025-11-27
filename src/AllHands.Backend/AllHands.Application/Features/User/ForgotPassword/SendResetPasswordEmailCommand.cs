namespace AllHands.Application.Features.User.ForgotPassword;

public sealed record SendResetPasswordEmailCommand(string Email, string FirstName, string Token);
