using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AllHands.Shared.WebApi.Rest.Auth;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(HasPermissionAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            return await base.GetPolicyAsync(policyName);
        var permissionName = policyName[HasPermissionAttribute.PolicyPrefix.Length..];
            
        var policy = new AuthorizationPolicyBuilder();
        policy.AddRequirements(new PermissionRequirement(permissionName));
        return policy.Build();
    }
}