using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.Update;

public sealed record UpdateRoleCommand(string Name, bool IsDefault, IReadOnlyList<string> Permissions)
    : RoleCommandBase(Name, Permissions), IRequest
{
    public Guid Id { get; set; }
}
