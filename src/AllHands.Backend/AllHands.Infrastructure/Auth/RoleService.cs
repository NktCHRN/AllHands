using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Application.Features.Roles.Create;
using AllHands.Application.Features.Roles.Get;
using AllHands.Application.Features.Roles.GetById;
using AllHands.Application.Features.Roles.GetUsersInRole;
using AllHands.Application.Features.Roles.Update;
using AllHands.Domain.Exceptions;
using AllHands.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllHands.Infrastructure.Auth;

public sealed class RoleService(ICurrentUserService currentUserService, AuthDbContext dbContext, RoleManager<AllHandsRole> roleManager, TimeProvider timeProvider) : IRoleService
{
    public async Task<IReadOnlyList<RoleWithUsersCountDto>> GetAsync(CancellationToken cancellationToken)
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

    public async Task<GetRoleByIdResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();

        var role = await dbContext.Roles
            .Include(r => r.Claims.Where(c => c.ClaimType == AuthConstants.PermissionClaimName))
            .AsNoTracking()
            .Where(r => r.CompanyId == companyId && r.Id == id)
            .Select(r => new {Role = r, UsersCount = r.Users.Count(u => !u.User!.DeletedAt.HasValue)})
            .FirstOrDefaultAsync(cancellationToken);

        if (role is null)
        {
            return null;
        }

        return new GetRoleByIdResult(
            role.Role.Id, 
            role.Role.Name ?? string.Empty, 
            role.Role.IsDefault, 
            role.UsersCount, 
            role.Role.Claims
                .Select(c => c.ClaimValue)
                .Order()
                .ToList());
    }

    public async Task<Guid> CreateAsync(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();

        var role = new AllHandsRole()
        {
            Id = Guid.CreateVersion7(),
            Name = command.Name,
            CompanyId = companyId,
            Claims = command.Permissions.Select(p => new AllHandsRoleClaim
            {
                Id = Guid.CreateVersion7(),
                ClaimType = AuthConstants.PermissionClaimName,
                ClaimValue = p
            }).ToList()
        };
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new EntityValidationFailedException(IdentityUtilities.IdentityErrorsToString(result.Errors));
        }
        
        return role.Id;
    }

    public async Task UpdateAsync(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();
        
        var role = await dbContext.Roles
            .Include(r => r.Claims.Where(c => c.ClaimType == AuthConstants.PermissionClaimName))
            .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.Id == command.Id, cancellationToken: cancellationToken)
            ?? throw new EntityNotFoundException("Role was not found");

        if (role.IsDefault && !command.IsDefault)
        {
            throw new EntityValidationFailedException("Make another role default first");
        }

        if (!role.IsDefault && command.IsDefault)
        {
            var defaultRole = await GetDefaultRoleAsync(companyId, cancellationToken);
            if (defaultRole != null)
            {
                defaultRole.IsDefault = false;
                dbContext.Roles.Update(defaultRole);
            }
        }
        
        role.Name = command.Name;
        role.IsDefault = command.IsDefault;
        
        var permissionsToRemove = role.Claims
            .ExceptBy(command.Permissions, c => c.ClaimValue)
            .ToList();
        dbContext.AllHandsRoleClaims.RemoveRange(permissionsToRemove);
        
        var permissionsToAdd = command.Permissions
            .Except(role.Claims.Select(c => c.ClaimValue))
            .ToList();
        foreach (var permission in permissionsToAdd)
        {
            dbContext.AllHandsRoleClaims.Add(new AllHandsRoleClaim()
            {
                Id = Guid.CreateVersion7(),
                ClaimType = AuthConstants.PermissionClaimName,
                ClaimValue = permission,
                RoleId = role.Id
            });
        }

        var updateSessionsTask = new RecalculateCompanySessionsTask()
        {
            Id = Guid.CreateVersion7(),
            CompanyId = companyId,
            RequestedAt = timeProvider.GetUtcNow(),
            RequesterUserId = currentUserService.GetId()
        };
        dbContext.RecalculateCompanySessionsTasks.Add(updateSessionsTask);
        
        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            throw new EntityValidationFailedException(IdentityUtilities.IdentityErrorsToString(result.Errors));
        }
    }

    private async Task<AllHandsRole?> GetDefaultRoleAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await dbContext.Roles
            .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.IsDefault, cancellationToken: cancellationToken);
    }
}
