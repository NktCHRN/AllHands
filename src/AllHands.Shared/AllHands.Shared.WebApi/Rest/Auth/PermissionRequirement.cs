using Microsoft.AspNetCore.Authorization;

namespace AllHands.Shared.WebApi.Rest.Auth;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
