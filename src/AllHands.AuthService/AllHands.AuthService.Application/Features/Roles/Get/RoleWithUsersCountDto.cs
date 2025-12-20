using AllHands.AuthService.Application.Dto;

namespace AllHands.AuthService.Application.Features.Roles.Get;

public sealed record RoleWithUsersCountDto(Guid Id, string Name, bool IsDefault, int UsersCount) : RoleDto(Id, Name, IsDefault);
