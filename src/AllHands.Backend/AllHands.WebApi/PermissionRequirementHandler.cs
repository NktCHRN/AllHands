using AllHands.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace AllHands.WebApi;

public sealed class PermissionRequirementHandler(IPermissionsContainer permissionsContainer) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissions = requirement.Permission.Replace(" ", string.Empty).Split(',');

        foreach (var permission in permissions)
        {
            if (context.User.IsAllowed(permissionsContainer, permission))       // "OR" - user has at least one of permissions. For "AND" use multiple attributes.
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        
        return Task.CompletedTask;
    }
}
