using System.Collections;
using System.Data;
using System.Security.Claims;
using AllHands.Application.Abstractions;
using AllHands.Application.Features.User.Login;
using AllHands.Application.Features.User.RegisterFromInvitation;
using AllHands.Domain.Exceptions;
using AllHands.Infrastructure.Abstractions;
using AllHands.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllHands.Infrastructure.Auth;

public sealed class AccountService(
    UserManager<AllHandsIdentityUser> userManager, 
    AuthDbContext dbContext, 
    IPermissionsContainer permissionsContainer, 
    IInvitationService invitationService) : IAccountService
{
    public async Task<LoginResult> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(login);
        var checkPasswordResult = user is not null && await userManager.CheckPasswordAsync(user, password);
        if (!checkPasswordResult)
        {
            throw new UserUnauthorizedException("Invalid login or password.");
        }
        
        var claimsPrincipal = await CreateClaimsPrincipalAsync(user!);
        
        return new LoginResult(claimsPrincipal);
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
            new Claim(AuthConstants.PermissionClaimName, permissionsString)
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

        var useInvitationResult = await invitationService.UseAsync(command.InvitationId, command.InvitationToken, cancellationToken);
        
        var user = await userManager.FindByIdAsync(useInvitationResult.UserId.ToString())
            ?? throw new InvalidOperationException("User was not found.");
        if (user.DeletedAt.HasValue)
        {
            throw new UserUnauthorizedException("Invalid invitation token.");
        }
        
        await userManager.AddPasswordAsync(user, command.Password);
        
        await transaction.CommitAsync(cancellationToken);

        return user.Id;
    }
}
