using MediatR;

namespace AllHands.Application.Features.User.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResult>;
