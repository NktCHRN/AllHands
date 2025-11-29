using AllHands.Application.Features.Roles.Get;

namespace AllHands.Application.Abstractions;

public interface IRoleService
{
    Task<IReadOnlyList<RoleWithUsersCountDto>> GetRolesAsync(CancellationToken cancellationToken);
}
