using MediatR;

namespace AllHands.AuthService.Application.Features.User.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResult>;
