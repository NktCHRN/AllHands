using AllHands.Shared.Application.Auth;
using AllHands.Shared.Domain.UserContext;

namespace AllHands.Shared.Infrastructure.Auth;

public sealed class UserPermissionService(IPermissionsContainer permissionsContainer, IUserContextAccessor userContextAccessor) : IUserPermissionService
{
    private Domain.UserContext.UserContext? UserContext => userContextAccessor.UserContext;
    
    public bool IsAllowed(string permission)
    {
        if (UserContext == null)
        {
            return false;
        }
        
        var permissionFound = permissionsContainer.Permissions.TryGetValue(permission, out var permissionIndex);

        if (!permissionFound || permissionIndex >= UserContext.Permissions.Length)
        {
            return false;
        }
        
        return UserContext.Permissions[permissionIndex];
    }

    public IReadOnlyList<string> GetPermissions()
    {
        if (UserContext == null)
        {
            return Array.Empty<string>();
        }
        
        var permissions = new List<string>();
        foreach (var (permission, index) in permissionsContainer.Permissions)
        {
            if (UserContext.Permissions.Length > index && UserContext.Permissions[index])
            {
                permissions.Add(permission);
            }
        }
        
        return permissions;
    }
}
