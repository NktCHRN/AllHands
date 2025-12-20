using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.Get;

public sealed record GetRolesQuery() : IRequest<GetRolesResult>;
