using MediatR;

namespace AllHands.AuthService.Application.Features.User.ResetPassword;

public sealed record ResetPasswordCommand(string Email) : IRequest;
