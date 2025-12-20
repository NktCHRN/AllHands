using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.GetById;

public sealed record GetRoleByIdQuery(Guid Id) : IRequest<GetRoleByIdResult>;
