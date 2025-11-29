using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.Roles.Create;

public sealed record CreateRoleCommand(string Name, IReadOnlyList<string> Permissions) : RoleCommandBase(Name, Permissions), IRequest<CreatedEntityDto>;
