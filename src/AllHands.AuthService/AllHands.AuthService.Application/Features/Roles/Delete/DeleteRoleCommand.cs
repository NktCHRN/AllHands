using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.Delete;

public sealed record DeleteRoleCommand(Guid Id) : IRequest;
