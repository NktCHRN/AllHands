using System.Data;
using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Application.Abstractions;
using AllHands.AuthService.Application.Features.Employees.Create;
using AllHands.AuthService.Application.Features.Employees.Update;
using AllHands.AuthService.Application.Features.Employees.UpdateRole;
using AllHands.AuthService.Application.Features.User.ChangePassword;
using AllHands.AuthService.Application.Features.User.Login;
using AllHands.AuthService.Application.Features.User.RegisterFromInvitation;
using AllHands.AuthService.Domain.Models;
using AllHands.AuthService.Infrastructure.Abstractions;
using AllHands.Shared.Contracts.Messaging.Events.Users;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Domain.Utilities;
using AllHands.Shared.Infrastructure.Messaging;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace AllHands.AuthService.Infrastructure.Auth;

public sealed class AccountService(
    UserManager<AllHandsIdentityUser> userManager, 
    AuthDbContext dbContext, 
    IUserClaimsFactory userClaimsFactory,
    IInvitationService invitationService,
    IUserContextAccessor userContextAccessor,
    IPasswordResetTokenProvider passwordResetTokenProvider,
    TimeProvider timeProvider,
    ITicketModifier ticketModifier,
    IDbContextOutbox messageBus) : IAccountService
{
    private IUserContext UserContext => userContextAccessor.UserContext ?? throw new InvalidOperationException("UserContext is not set.");
    
    public async Task<LoginResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = StringUtilities.GetNormalizedEmail(email);
        var globalUser = await dbContext.GlobalUsers
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        if (globalUser is null)
        {
            throw new UserUnauthorizedException("Invalid login or password.");
        }
        
        var user = await userManager.FindByNameAsync(GetUserName(globalUser.Email, globalUser.DefaultCompanyId));
        if (user is null)
        {
            user = await userManager.FindByEmailAsync(globalUser.Email);
            if (user is not null)
            {
                globalUser.DefaultCompanyId = user.CompanyId;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        var checkPasswordResult = user is not null 
                                  && !user.DeletedAt.HasValue 
                                  && user.IsInvitationAccepted
                                  && await userManager.CheckPasswordAsync(user, password);
        if (!checkPasswordResult)
        {
            throw new UserUnauthorizedException("Invalid login or password.");
        }
        
        var userWithClaims = await dbContext.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.Role)
            .ThenInclude(r => r!.Claims)
            .Where(u => u.Id == user!.Id)
            .FirstAsync(cancellationToken: cancellationToken);
        var claimsPrincipal = userClaimsFactory.CreateClaimsPrincipal(userWithClaims);
        
        return new LoginResult(claimsPrincipal);
    }
    
    public async Task ReloginAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var currentUserId = UserContext.Id;
        
        var currentUser = await dbContext.Users
            .Include(u => u.GlobalUser)
                .ThenInclude(g => g!.Users)
            .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
        if (currentUser is null)
        {
            throw new InvalidOperationException("User was not found.");
        }

        if (currentUser.GlobalUser!.Users.All(u => u.CompanyId != companyId))
        {
            throw new UserUnauthorizedException("Company was not found.");
        }
        
        currentUser.GlobalUser.DefaultCompanyId = companyId;
        await dbContext.SaveChangesAsync(cancellationToken);
    }


    public async Task<Guid> RegisterFromInvitationAsync(RegisterFromInvitationCommand command, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
        
        var user = await dbContext.Users
            .Include(u => u.GlobalUser)
                .ThenInclude(g => g!.Users.Where(u => !u.DeletedAt.HasValue))
            .Where(u => u.Invitations.Any(i => i.Id == command.InvitationId))
            .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException("User was not found.");
        
        if (user.DeletedAt.HasValue)
        {
            throw new UserUnauthorizedException("Invalid invitation token.");
        }

        if (user.IsInvitationAccepted)
        {
            throw new UserUnauthorizedException("Invitation is already accepted.");
        }

        if (user.GlobalUser!.Users.Count > 1)
        {
            var existingUser = user.GlobalUser.Users.First(u => u.Id != user.Id);
            var isValidPasswordAsync = await userManager.CheckPasswordAsync(existingUser, command.Password);
            if (!isValidPasswordAsync)
            {
                throw new UserUnauthorizedException("Incorrect password.");
            }
        }
        else
        {
            var identityResult = await userManager.AddPasswordAsync(user, command.Password);
            if (!identityResult.Succeeded)
            {
                throw new EntityValidationFailedException(IdentityUtilities.IdentityErrorsToString(identityResult.Errors));
            }
        }
        
        user.IsInvitationAccepted = true;
        user.GlobalUser.DefaultCompanyId = user.CompanyId;
        await invitationService.UseAsync(command.InvitationId, invitationToken: command.InvitationToken, cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return user.Id;
    }

    private static string GetUserName(string email, Guid companyId)
    {
        return $"{email}_{companyId}";
    }

    public async Task GenerateResetPasswordToken(string email, CancellationToken cancellationToken)
    {
        var currentDateTime = timeProvider.GetUtcNow();
        var normalizedEmail = StringUtilities.GetNormalizedEmail(email);
        var globalUser = await dbContext.GlobalUsers
            .Include(g => g.Users)
            .Include(x => x.PasswordResetTokens.Where(t => t.ExpiresAt >= currentDateTime))
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        if (globalUser is null)
        {
            return;
        }

        var maxLastGenerationTime = currentDateTime.Add(-passwordResetTokenProvider.Options.TokenRecreationTimeout);
        var alreadyExistingToken = globalUser.PasswordResetTokens.FirstOrDefault(t => t.IssuedAt > maxLastGenerationTime);
        if (alreadyExistingToken is not null)
        {
            throw new EntityAlreadyExistsException($"Please, wait {(alreadyExistingToken.IssuedAt - maxLastGenerationTime).Humanize()} to generate new password reset token.");
        }
        
        var (token, entity) = passwordResetTokenProvider.Generate(globalUser.Id);
        
        await dbContext.PasswordResetTokens.AddAsync(entity, cancellationToken);

        messageBus.Enroll(dbContext);
        await messageBus.PublishWithHeadersAsync(new ResetPasswordRequestedEvent(globalUser.Email, globalUser.Users.FirstOrDefault(u => u.CompanyId == globalUser.DefaultCompanyId)?.FirstName
            ?? globalUser.Users.First().FirstName, token), UserContext);

        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }

    public async Task ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
        
        var currentDateTime = timeProvider.GetUtcNow();
        var normalizedEmail = StringUtilities.GetNormalizedEmail(command.Email);
        var globalUser = await dbContext.GlobalUsers
            .Include(g => g.Users.Where(u => u.IsInvitationAccepted))
            .Include(x => x.PasswordResetTokens.Where(t => t.ExpiresAt >= currentDateTime && !t.IsUsed))
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        if (globalUser is null || globalUser.Users.Count == 0)
        {
            throw new EntityNotFoundException("User was not found");
        }
        
        var token = globalUser.PasswordResetTokens.FirstOrDefault(t => passwordResetTokenProvider.Verify(command.Token, t.TokenHash));
        if (token is null)
        {
            throw new UserUnauthorizedException("Invalid token.");
        }

        token.IsUsed = true;

        foreach (var user in globalUser.Users)
        {
            await userManager.RemovePasswordAsync(user);
            var identityResult = await userManager.AddPasswordAsync(user, command.NewPassword);
            if (!identityResult.Succeeded)
            {
                throw new EntityValidationFailedException(IdentityUtilities.IdentityErrorsToString(identityResult.Errors));
            }
            
            await ticketModifier.ExpireActiveSessionsAsync(dbContext, user.Id, cancellationToken);
        }
        
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<CreateEmployeeAccountResult> CreateAsync(CreateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
        var companyId = UserContext.CompanyId;
        var normalizedEmail = StringUtilities.GetNormalizedEmail(command.Email);

        var globalUser = await GetOrCreateGlobalUserByEmailAsync(command.Email, companyId, cancellationToken);
        
        var defaultRole = await dbContext.Roles
            .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.IsDefault, cancellationToken)
            ?? throw new EntityNotFoundException("Default role was not found.");

        var user = new AllHandsIdentityUser()
        {
            Id = Guid.CreateVersion7(),
            FirstName = command.FirstName,
            MiddleName = command.MiddleName,
            LastName = command.LastName,
            PhoneNumber = command.PhoneNumber,
            Email = command.Email,
            NormalizedEmail = normalizedEmail,
            CompanyId = companyId,
            UserName = GetUserName(command.Email, companyId),
            GlobalUser = globalUser,
            Roles = new List<AllHandsUserRole>()
            {
                new AllHandsUserRole()
                {
                    RoleId = defaultRole.Id
                }
            },
            EmployeeId = command.EmployeeId,
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            throw new EntityValidationFailedException(IdentityUtilities.IdentityErrorsToString(result.Errors));
        }

        var invitation = await invitationService.CreateAsync(user.Id, UserContext.Id, cancellationToken);
        
        messageBus.Enroll(dbContext);
        await messageBus.PublishWithHeadersAsync(new UserInvitedEvent(
            user.Email, 
            user.FirstName, 
            StringUtilities.GetFullName(UserContext.FirstName, UserContext.MiddleName, UserContext.LastName),
            invitation.Id,
            invitation.Token), UserContext);
        await messageBus.PublishWithHeadersAsync(new UserCreatedEvent(user.Id, user.GlobalUserId, user.Roles.Select(r => r.RoleId).ToList(), companyId), UserContext);
        
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return new CreateEmployeeAccountResult(user.Id, defaultRole.Id, globalUser.Id);
    }

    public async Task RegenerateInvitationAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        var user = await dbContext.Users
                       .AsNoTracking()
                       .FirstOrDefaultAsync(u => u.EmployeeId == employeeId, cancellationToken)
                   ?? throw new EntityNotFoundException("User was not found");
        var invitation = await invitationService.CreateAsync(user.Id, UserContext.Id, cancellationToken);
        messageBus.Enroll(dbContext);
        await messageBus.PublishWithHeadersAsync(new UserInvitedEvent(
            user.Email ?? string.Empty, 
            user.FirstName, 
            StringUtilities.GetFullName(UserContext.FirstName, UserContext.MiddleName, UserContext.LastName),
            invitation.Id,
            invitation.Token), UserContext);
        
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task UpdateAsync(UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

        var user = await dbContext.Users
            .Include(u => u.Roles)
                .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken)
            ?? throw new EntityNotFoundException("User was not found");
        
        user.FirstName = command.FirstName;
        user.MiddleName = command.MiddleName;
        user.LastName = command.LastName;
        user.PhoneNumber = command.PhoneNumber;

        if (user.Email != command.Email)
        {
            var newGlobalUser = await GetOrCreateGlobalUserByEmailAsync(command.Email, user.CompanyId, cancellationToken);
            user.GlobalUserId = newGlobalUser.Id;
            
            user.Email = command.Email;
            user.NormalizedEmail = StringUtilities.GetNormalizedEmail(command.Email);
            user.UserName = GetUserName(command.Email, user.CompanyId);
        }
        
        await userManager.UpdateAsync(user);
        
        messageBus.Enroll(dbContext);
        await messageBus.PublishWithHeadersAsync(new UserSessionsRecalculationRequestedEvent(command.UserId, UserContext.Id), UserContext);
        await messageBus.PublishWithHeadersAsync(new UserUpdatedEvent(user.Id, user.GlobalUserId, user.Roles.Select(r => r.RoleId).ToList(), true, user.CompanyId), UserContext);
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
    }
    
    public async Task UpdateRoleAsync(UpdateEmployeeRoleCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

        var user = await dbContext.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken)
            ?? throw new EntityNotFoundException("User was not found");

        if (user.Roles.FirstOrDefault()?.RoleId != command.RoleId)
        {
            dbContext.RemoveRange(user.Roles);
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == command.RoleId, cancellationToken)
                ?? throw new EntityNotFoundException("Role was not found");
            user.Roles.Add(new AllHandsUserRole()
            {
                RoleId = role.Id
            });
        }
        
        await userManager.UpdateAsync(user);

        messageBus.Enroll(dbContext);
        await messageBus.PublishWithHeadersAsync(new UserSessionsRecalculationRequestedEvent(command.UserId, UserContext.Id), UserContext);
        await messageBus.PublishWithHeadersAsync(new UserUpdatedEvent(user.Id, user.GlobalUserId, user.Roles.Select(r => r.RoleId).ToList(), true, user.CompanyId), UserContext);
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<AllHandsGlobalUser> GetOrCreateGlobalUserByEmailAsync(string email, Guid companyId,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = StringUtilities.GetNormalizedEmail(email);
        var globalUser = await dbContext.GlobalUsers.FirstOrDefaultAsync(g => g.NormalizedEmail == normalizedEmail, cancellationToken);

        if (globalUser is null)
        {
            globalUser = new AllHandsGlobalUser()
            {
                Id = Guid.CreateVersion7(),
                Email = email,
                NormalizedEmail = normalizedEmail,
                DefaultCompanyId = companyId
            };
            dbContext.GlobalUsers.Add(globalUser);
        }
        
        return globalUser;
    }

    public async Task DeactivateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
                       .Include(u => u.Roles)
                       .ThenInclude(r => r.Role)
                       .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new EntityNotFoundException("User was not found");

        user.DeactivatedAt = timeProvider.GetUtcNow();
        
        messageBus.Enroll(dbContext);
        await ticketModifier.ExpireActiveSessionsAsync(dbContext, userId, cancellationToken);
        
        await messageBus.PublishWithHeadersAsync(new UserUpdatedEvent(user.Id, user.GlobalUserId, user.Roles.Select(r => r.RoleId).ToList(), false, user.CompanyId), UserContext);
        
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }
    
    public async Task ReactivateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
                       .IgnoreQueryFilters()
                       .Include(u => u.Roles)
                       .ThenInclude(r => r.Role)
                       .FirstOrDefaultAsync(u => u.Id == userId && !u.DeletedAt.HasValue, cancellationToken)
                   ?? throw new EntityNotFoundException("User was not found");

        user.DeactivatedAt = null;
        if (!user.Roles.Any() || user.Roles.All(r => r.Role?.DeletedAt.HasValue == true))
        {
            var companyId = UserContext.CompanyId;
            var defaultRole = await dbContext.Roles
                                  .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.IsDefault, cancellationToken)
                              ?? throw new EntityNotFoundException("Default role was not found.");
            user.Roles.Add(new AllHandsUserRole()
            {
                RoleId = defaultRole.Id
            });
        }
        
        messageBus.Enroll(dbContext);
        
        await messageBus.PublishWithHeadersAsync(new UserUpdatedEvent(user.Id, user.GlobalUserId, user.Roles.Select(r => r.RoleId).ToList(), true, user.CompanyId), UserContext);
        await messageBus.PublishWithHeadersAsync(
            new UserReactivatedEvent(user.Id, user.GlobalUserId, user.Roles.Select(r => r.RoleId).ToList(),
                user.CompanyId), UserContext);
        
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
                       .IgnoreQueryFilters()
                       .FirstOrDefaultAsync(u => u.Id == userId && !u.DeletedAt.HasValue, cancellationToken)
                   ?? throw new EntityNotFoundException("User was not found");
        
        user.DeletedAt = timeProvider.GetUtcNow();
        
        await ticketModifier.ExpireActiveSessionsAsync(dbContext, userId, cancellationToken);
        
        messageBus.Enroll(dbContext);
        await messageBus.PublishWithHeadersAsync(new UserDeletedEvent(user.Id, user.CompanyId), UserContext);
        
        await messageBus.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }
}
