using MediatR;

namespace AllHands.Application.Features.Roles.Get;

public sealed record GetRolesQuery() : IRequest<GetRolesResult>;
