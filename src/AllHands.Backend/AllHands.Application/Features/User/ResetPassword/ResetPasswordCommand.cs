using MediatR;

namespace AllHands.Application.Features.User.ResetPassword;

public sealed record ResetPasswordCommand(string Email) : IRequest;
