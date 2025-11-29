using AllHands.Application.Dto;

namespace AllHands.Application.Features.Roles.Get;

public sealed record RoleWithUsersCountDto(string Name, bool IsDefault, int UsersCount) : RoleDto(Name, IsDefault);
