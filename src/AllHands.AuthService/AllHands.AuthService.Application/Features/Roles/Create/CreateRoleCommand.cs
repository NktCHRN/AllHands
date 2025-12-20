using AllHands.Shared.Application.Dto;
using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.Create;

public sealed record CreateRoleCommand(string Name, IReadOnlyList<string> Permissions) : RoleCommandBase(Name, Permissions), IRequest<CreatedEntityDto>;
