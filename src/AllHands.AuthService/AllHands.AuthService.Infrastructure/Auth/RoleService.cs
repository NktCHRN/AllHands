using System.Data;
using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Application.Abstractions;
using AllHands.AuthService.Application.Dto;
using AllHands.AuthService.Application.Features.Roles.Create;
using AllHands.AuthService.Application.Features.Roles.Get;
using AllHands.AuthService.Application.Features.Roles.GetById;
using AllHands.AuthService.Application.Features.Roles.GetUsersInRole;
using AllHands.AuthService.Application.Features.Roles.Update;
using AllHands.AuthService.Domain.Models;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Contracts.Messaging.Events.Roles;
using AllHands.Shared.Contracts.Messaging.Events.Users;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Infrastructure.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace AllHands.AuthService.Infrastructure.Auth;

public sealed class RoleService(IUserContextAccessor userContextAccessor, AuthDbContext dbContext, RoleManager<AllHandsRole> roleManager, TimeProvider timeProvider, IDbContextOutbox  messageBus) : IRoleService
{
    private IUserContext UserContext => userContextAccessor.UserContext ?? throw new InvalidOperationException("UserContext is not set");
    
    public async Task<IReadOnlyList<RoleWithUsersCountDto>> GetAsync(CancellationToken cancellationToken)
    {
        var companyId = UserContext.CompanyId;

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

    public async Task<PagedDto<EmployeeTitleDto>> GetUsersAsync(GetUsersInRoleQuery query, CancellationToken cancellationToken)
    {
        var companyId = UserContext.CompanyId;

        var dbQuery = dbContext.Users
            .AsNoTracking()
            .Where(u => u.CompanyId == companyId && u.Roles.Any(r => r.RoleId == query.RoleId));
        
        var count = await dbQuery.CountAsync(cancellationToken);
        
        var users = await dbQuery
            .OrderByDescending(u => u.Id)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        return new PagedDto<EmployeeTitleDto>(
            users
                .Select(u => new EmployeeTitleDto
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
        var companyId = UserContext.CompanyId;

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
        var companyId = UserContext.CompanyId;
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

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
            }).ToList(),
            CreatedAt = timeProvider.GetUtcNow(),
            CreatedByUserId = UserContext.Id
        };

        messageBus.Enroll(dbContext);
        await messageBus.PublishWithHeadersAsync(new RoleCreatedEvent(role.Id, role.Name, role.CompanyId, role.IsDefault), UserContext);
        
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
        
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new EntityValidationFailedException(IdentityUtilities.IdentityErrorsToString(result.Errors));
        }
        
        await transaction.CommitAsync(cancellationToken);
        
        return role.Id;
    }

    public async Task UpdateAsync(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var companyId = UserContext.CompanyId;
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
        
        messageBus.Enroll(dbContext);
        
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
                await messageBus.PublishWithHeadersAsync(new RoleUpdatedEvent(defaultRole.Id, defaultRole.Name ?? string.Empty, defaultRole.CompanyId, false), UserContext);
            }
        }
        
        role.Name = command.Name;
        role.IsDefault = command.IsDefault;
        role.UpdatedAt = timeProvider.GetUtcNow();
        role.UpdatedByUserId = UserContext.Id;
        
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
        
        await messageBus.PublishWithHeadersAsync(new RoleUpdatedEvent(role.Id, role.Name, role.CompanyId, role.IsDefault), UserContext);
        
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
        
        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            throw new EntityValidationFailedException(IdentityUtilities.IdentityErrorsToString(result.Errors));
        }
        
        await transaction.CommitAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var companyId = UserContext.CompanyId;
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
        
        var role = await dbContext.Roles
            .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.Id == id, cancellationToken: cancellationToken)
            ?? throw new EntityNotFoundException("Role was not found");

        if (role.IsDefault)
        {
            throw new EntityValidationFailedException("Make another role default first");
        }
        
        var defaultRole = await GetDefaultRoleAsync(companyId, cancellationToken);
        
        role.DeletedAt = timeProvider.GetUtcNow();
        role.DeletedByUserId = UserContext.Id;
        
        messageBus.Enroll(dbContext);
        await messageBus.PublishWithHeadersAsync(new RoleDeletedEvent(role.Id, defaultRole?.Id ?? Guid.Empty, role.CompanyId), UserContext);
        
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task ResetUsersRoleAsync(Guid oldRoleId, CancellationToken cancellationToken)
    {
        var defaultRole = await GetDefaultRoleAsync(oldRoleId, cancellationToken);
        if (defaultRole is null)
        {
            throw new EntityNotFoundException("Default role was not found");
        }

        var role = await dbContext.Roles
                       .IgnoreQueryFilters()
                       .Include(r => r.Users.Where(u => !u.User!.DeletedAt.HasValue && !u.User.DeactivatedAt.HasValue))
                       .ThenInclude(u => u.User)
                       .FirstOrDefaultAsync(r => r.Id == oldRoleId, cancellationToken: cancellationToken)
                   ?? throw new EntityNotFoundException("Role was not found");
        
        messageBus.Enroll(dbContext);
        foreach (var userRole in role.Users)
        {
            dbContext.UserRoles.Remove(userRole);
            dbContext.UserRoles.Add(new AllHandsUserRole()
            {
                UserId = userRole.UserId,
                RoleId = defaultRole.Id
            });
            var user = userRole.User;
            await messageBus.PublishWithHeadersAsync(
                new UserUpdatedEvent(user!.Id, user.GlobalUserId, [defaultRole.Id], true, role.CompanyId), UserContext);
        }
        
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }

    private async Task<AllHandsRole?> GetDefaultRoleAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await dbContext.Roles
            .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.IsDefault, cancellationToken: cancellationToken);
    }
}
