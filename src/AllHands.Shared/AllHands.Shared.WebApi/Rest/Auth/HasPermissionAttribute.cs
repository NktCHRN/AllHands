using Microsoft.AspNetCore.Authorization;

namespace AllHands.Shared.WebApi.Rest.Auth;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "PERMISSION:";

    public HasPermissionAttribute(string permission)
    {
        Policy = $"{PolicyPrefix}{permission}";
    }
}
