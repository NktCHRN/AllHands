namespace AllHands.Application.Features.User.ResetPassword;

public record GenerateResetPasswordTokenResult(bool IsSuccess, string? Token = null, string? FirstName = null);
