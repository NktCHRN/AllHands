using AllHands.Shared.Application.Auth;
using Microsoft.AspNetCore.Authorization;

namespace AllHands.Shared.WebApi.Rest.Auth;

public sealed class PermissionRequirementHandler(IUserPermissionService userPermissionService) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissions = requirement.Permission.Replace(" ", string.Empty).Split(',');

        foreach (var permission in permissions)
        {
            if (userPermissionService.IsAllowed(permission))       // "OR" - user has at least one of permissions. For "AND" use multiple attributes.
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        
        return Task.CompletedTask;
    }
}
