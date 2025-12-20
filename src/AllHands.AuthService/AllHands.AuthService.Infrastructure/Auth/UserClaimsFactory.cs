using System.Collections;
using System.Security.Claims;
using AllHands.AuthService.Application.Constants;
using AllHands.AuthService.Domain.Models;
using AllHands.Shared.Application.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AllHands.AuthService.Infrastructure.Auth;

public sealed class UserClaimsFactory(IPermissionsContainer permissionsContainer) : IUserClaimsFactory
{
    public ClaimsPrincipal CreateClaimsPrincipal(AllHandsIdentityUser user)
    {
        var claims = CreateClaims(user);

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        return new ClaimsPrincipal(claimsIdentity);
    }

    public IReadOnlyList<Claim> CreateClaims(AllHandsIdentityUser user)
    {
        var permissionsString = GetPermissionsString(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim("middlename", user.MiddleName ?? string.Empty),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(AuthConstants.PermissionClaimName, permissionsString),
            new Claim("companyid", user.CompanyId.ToString()),
            new Claim(AllHandsClaimTypes.EmployeeId, user.EmployeeId.ToString())
        };

        foreach (var role in user.Roles.Select(r => r.Role!))
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name ?? string.Empty));
        }
        
        return claims;
    }

    private string GetPermissionsString(AllHandsIdentityUser userWithClaims)
    {
        var bitArray = new BitArray(permissionsContainer.BitArrayLength);

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
