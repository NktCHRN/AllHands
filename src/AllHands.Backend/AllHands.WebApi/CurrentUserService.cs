using System.Collections;
using System.Security.Claims;
using AllHands.Application.Abstractions;
using AllHands.Infrastructure.Abstractions;

namespace AllHands.WebApi;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor, IPermissionsContainer permissionsContainer) : ICurrentUserService
{
    private HttpContext HttpContext 
        => httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not accessible.");
    private ClaimsPrincipal User => HttpContext.User;
    
    public Guid GetId()
    {
        var parsed = Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid id);

        return parsed
            ? id
            : throw new InvalidOperationException("Invalid user identifier.");
    }

    public string GetEmail()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? throw new InvalidOperationException("Invalid user email.");
    }

    public string? GetPhoneNumber()
    {
        return User.FindFirst(ClaimTypes.MobilePhone)?.Value;
    }

    public CurrentUserDto GetCurrentUser()
    {
        return new CurrentUserDto(
            GetId(), 
            GetEmail(), 
            GetPhoneNumber(),
            User.FindFirst(ClaimTypes.GivenName)?.Value ?? throw new InvalidOperationException("Invalid user first name."),
            User.FindFirst("middle_name")?.Value ?? throw new InvalidOperationException("Invalid user middle name."),
            User.FindFirst(ClaimTypes.Surname)?.Value ?? throw new InvalidOperationException("Invalid user last name."));
    }

    public bool IsAllowed(string permission)
    {
        var claim = User.FindFirst("permissions")?.Value ??
                    throw new InvalidOperationException("Invalid user permissions.");
        var permissions = new BitArray(Convert.FromBase64String(claim));

        return permissions[permissionsContainer.Permissions[permission]];
    }
}
