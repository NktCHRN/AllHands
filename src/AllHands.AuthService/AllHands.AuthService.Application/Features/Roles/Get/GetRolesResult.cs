namespace AllHands.AuthService.Application.Features.Roles.Get;

public sealed record GetRolesResult(IReadOnlyList<RoleWithUsersCountDto> Roles);
