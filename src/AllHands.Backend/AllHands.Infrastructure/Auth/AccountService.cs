using System.Collections;
using System.Data;
using System.Security.Claims;
using AllHands.Application.Abstractions;
using AllHands.Application.Features.User.ChangePassword;
using AllHands.Application.Features.User.Login;
using AllHands.Application.Features.User.RegisterFromInvitation;
using AllHands.Application.Features.User.ResetPassword;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Utilities;
using AllHands.Infrastructure.Abstractions;
using AllHands.Infrastructure.Auth.Entities;
using Humanizer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllHands.Infrastructure.Auth;

public sealed class AccountService(
    UserManager<AllHandsIdentityUser> userManager, 
    AuthDbContext dbContext, 
    IPermissionsContainer permissionsContainer, 
    IInvitationService invitationService,
    ICurrentUserService currentUserService,
    IPasswordResetTokenProvider passwordResetTokenProvider,
    TimeProvider timeProvider) : IAccountService
{
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
        
        var claimsPrincipal = await CreateClaimsPrincipalAsync(user!);
        
        return new LoginResult(claimsPrincipal);
    }
    
    public async Task ReloginAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserService.GetId();
        
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

    private async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(AllHandsIdentityUser user)
    {
        var userWithClaims = await dbContext.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.Role)
            .ThenInclude(r => r!.Claims)
            .Where(u => u.Id == user.Id)
            .FirstAsync();
        
        var permissionsString = GetPermissionsString(userWithClaims);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(AuthConstants.PermissionClaimName, permissionsString),
            new Claim("companyid", user.CompanyId.ToString()),
        };

        if (!string.IsNullOrEmpty(user.MiddleName))
        {
            claims.Add(new Claim("middlename", user.MiddleName));
        }

        foreach (var role in userWithClaims.Roles.Select(r => r.Role!))
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name ?? string.Empty));
        }

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        return new ClaimsPrincipal(claimsIdentity);
    }

    private string GetPermissionsString(AllHandsIdentityUser userWithClaims)
    {
        var bitArray = new BitArray(permissionsContainer.PermissionsLength);

        var permissionClaims = userWithClaims
            .Roles
            .SelectMany(r => r.Role!.Claims)
            .Where(c => c.ClaimType == AuthConstants.PermissionClaimName);
        foreach (var claim in permissionClaims)
        {
            var permissionName = claim.ClaimValue ?? throw new InvalidOperationException("Permission name cannot be null.");
            bitArray[permissionsContainer.Permissions[permissionName]] = true;
        }
        
        var byteArray = ToByteArray(bitArray);
        
        return Convert.ToBase64String(byteArray);
    }
    
    private static byte[] ToByteArray(BitArray bits)
    {
        var numBytes = (bits.Length + 7) / 8; 
        var bytes = new byte[numBytes];
        
        for (var i = 0; i < bits.Length; i++)
        {
            if (bits[i])
            {
                bytes[i / 8] |= (byte)(1 << (i % 8)); 
            }
        }
        return bytes;
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

    public async Task<GenerateResetPasswordTokenResult> GenerateResetPasswordToken(string email, CancellationToken cancellationToken)
    {
        var currentDateTime = timeProvider.GetUtcNow();
        var normalizedEmail = StringUtilities.GetNormalizedEmail(email);
        var globalUser = await dbContext.GlobalUsers
            .Include(g => g.Users)
            .Include(x => x.PasswordResetTokens.Where(t => t.ExpiresAt >= currentDateTime))
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        if (globalUser is null)
        {
            return new GenerateResetPasswordTokenResult(false);
        }

        var maxLastGenerationTime = currentDateTime.Add(-passwordResetTokenProvider.Options.TokenRecreationTimeout);
        var alreadyExistingToken = globalUser.PasswordResetTokens.FirstOrDefault(t => t.IssuedAt > maxLastGenerationTime);
        if (alreadyExistingToken is not null)
        {
            throw new EntityAlreadyExistsException($"Please, wait {(alreadyExistingToken.IssuedAt - maxLastGenerationTime).Humanize()} to generate new password reset token.");
        }
        
        var (token, entity) = passwordResetTokenProvider.Generate(globalUser.Id);
        
        await dbContext.PasswordResetTokens.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new GenerateResetPasswordTokenResult(
            true, 
            token, 
            globalUser.Users.FirstOrDefault(u => u.CompanyId == globalUser.DefaultCompanyId)?.FirstName
            ?? globalUser.Users.First().FirstName);
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
        }
        
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetUserIds(Guid currentUserId)
    {
        var users = await dbContext.Users
            .Where(u => u.GlobalUser!.Users.Any(gu => gu.Id == currentUserId) 
                        && !u.DeletedAt.HasValue 
                        && u.IsInvitationAccepted)
            .ToListAsync();
        
        return users.Select(u => u.Id).ToList();
    }
}
