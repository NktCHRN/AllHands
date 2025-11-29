using MediatR;

namespace AllHands.Application.Features.Roles.Delete;

public sealed record DeleteRoleCommand(Guid Id) : IRequest;
