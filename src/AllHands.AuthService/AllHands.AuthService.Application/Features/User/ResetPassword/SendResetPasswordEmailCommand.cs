namespace AllHands.AuthService.Application.Features.User.ResetPassword;

public sealed record SendResetPasswordEmailCommand(string Email, string FirstName, string Token);
