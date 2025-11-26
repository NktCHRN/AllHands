using AllHands.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace AllHands.WebApi;

public sealed class PermissionRequirementHandler(IPermissionsContainer permissionsContainer) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var result = context.User.IsAllowed(permissionsContainer, requirement.Permission);

        if (result)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
