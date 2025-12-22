using System.Collections;
using System.Security.Claims;
using AllHands.Application.Abstractions;
using AllHands.Infrastructure.Auth;

namespace AllHands.WebApi;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAllowed(this ClaimsPrincipal user, IPermissionsContainer permissionsContainer, string permission)
    {
        var claim = user.FindFirst(AuthConstants.PermissionClaimName)?.Value;
        if (string.IsNullOrEmpty(claim))
        {
            return false;
        }
        
        var permissions = new BitArray(Convert.FromBase64String(claim));

        var permissionFound = permissionsContainer.Permissions.TryGetValue(permission, out var permissionIndex);

        if (!permissionFound || permissionIndex >= permissions.Length)
        {
            return false;
        }
        
        return permissions[permissionIndex];
    }
}
