using MediatR;

namespace AllHands.Application.Features.User.Get;

public sealed record GetUserQuery() : IRequest<GetUserResult>;
