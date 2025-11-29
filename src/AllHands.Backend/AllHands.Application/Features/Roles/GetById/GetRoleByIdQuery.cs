using MediatR;

namespace AllHands.Application.Features.Roles.GetById;

public sealed record GetRoleByIdQuery(Guid Id) : IRequest<GetRoleByIdResult>;
