using System.Collections;
using System.Security.Claims;
using AllHands.Application.Abstractions;
using AllHands.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllHands.Infrastructure.Auth;

public sealed class AccountService(UserManager<AllHandsIdentityUser> userManager, AuthDbContext dbContext, IPermissionsContainer permissionsContainer) : IAccountService
{
    public async Task<LoginResult> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(login);
        var checkPasswordResult = user is not null && await userManager.CheckPasswordAsync(user, password);
        if (!checkPasswordResult)
        {
            return new LoginResult(false, null);
        }
        
        var claimsPrincipal = await CreateClaimsPrincipalAsync(user!);
        
        return new LoginResult(true, claimsPrincipal);
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
}
