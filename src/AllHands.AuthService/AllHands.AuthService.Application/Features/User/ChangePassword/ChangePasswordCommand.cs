using MediatR;

namespace AllHands.AuthService.Application.Features.User.ChangePassword;

public sealed record ChangePasswordCommand(string Email, string Token, string NewPassword) : IRequest;
