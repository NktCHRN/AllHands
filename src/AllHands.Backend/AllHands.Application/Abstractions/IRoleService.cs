using AllHands.Application.Dto;
using AllHands.Application.Features.Roles.Get;
using AllHands.Application.Features.Roles.GetById;
using AllHands.Application.Features.Roles.GetUsersInRole;

namespace AllHands.Application.Abstractions;

public interface IRoleService
{
    Task<IReadOnlyList<RoleWithUsersCountDto>> GetRolesAsync(CancellationToken cancellationToken);
    Task<PagedDto<UserDto>> GetUsersAsync(GetUsersInRoleQuery query, CancellationToken cancellationToken);
    Task<GetRoleByIdResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
