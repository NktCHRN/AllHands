using AllHands.Application.Abstractions;
using AllHands.Application.Features.Roles.Get;
using Microsoft.EntityFrameworkCore;

namespace AllHands.Infrastructure.Auth;

public sealed class RoleService(ICurrentUserService currentUserService, AuthDbContext dbContext) : IRoleService
{
    public async Task<IReadOnlyList<RoleWithUsersCountDto>> GetRolesAsync(CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();

        var roles = await dbContext.Roles
            .AsNoTracking()
            .Where(r => r.CompanyId == companyId)
            .Select(r => new {Role = r, UsersCount = r.Users.Count(u => !u.User!.DeletedAt.HasValue)})
            .OrderBy(r => r.Role.NormalizedName)
            .ToListAsync(cancellationToken);

        return roles
            .Select(r => new RoleWithUsersCountDto(r.Role.Name ?? string.Empty, r.Role.IsDefault, r.UsersCount))
            .ToList();
    }
}
