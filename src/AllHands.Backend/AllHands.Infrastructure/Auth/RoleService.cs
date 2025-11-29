using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Application.Features.Roles.Get;
using AllHands.Application.Features.Roles.GetUsersInRole;
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
            .Select(r => new RoleWithUsersCountDto(r.Role.Id, r.Role.Name ?? string.Empty, r.Role.IsDefault, r.UsersCount))
            .ToList();
    }

    public async Task<PagedDto<UserDto>> GetUsersAsync(GetUsersInRoleQuery query, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();

        var dbQuery = dbContext.Users
            .AsNoTracking()
            .Where(u => u.CompanyId == companyId && u.Roles.Any(r => r.RoleId == query.RoleId));
        
        var count = await dbQuery.CountAsync(cancellationToken);
        
        var users = await dbQuery
            .OrderByDescending(u => u.Id)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new PagedDto<UserDto>(
            users
                .Select(u => new UserDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    Email = u.Email!,
                    PhoneNumber = u.PhoneNumber
                }).ToList(),
            count);
    }
}
