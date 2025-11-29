using System.Collections;
using System.Security.Claims;
using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Infrastructure.Auth;

namespace AllHands.WebApi;

public sealed class CurrentHttpUserService(IHttpContextAccessor httpContextAccessor, IPermissionsContainer permissionsContainer) : ICurrentUserService
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
        return User.FindFirst(ClaimTypes.Email)?.Value ?? throw new InvalidOperationException("Invalid user email.");
    }

    public string? GetPhoneNumber()
    {
        return User.FindFirst(ClaimTypes.MobilePhone)?.Value;
    }

    public Guid GetCompanyId()
    {
        var isParsed = TryGetCompanyId(out var companyId);
        
        return isParsed
            ? companyId
            : throw new InvalidOperationException("Invalid company id.");
    }

    public bool TryGetCompanyId(out Guid companyId)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            companyId = Guid.Empty;
            return false;
        }
        
        var isParsed = Guid.TryParse(User.FindFirst("companyid")?.Value ?? string.Empty, out companyId);

        return isParsed;
    }

    public CurrentUserDto GetCurrentUser()
    {
        return new CurrentUserDto(
            GetId(), 
            GetEmail(), 
            GetPhoneNumber(),
            User.FindFirst(ClaimTypes.GivenName)?.Value ?? throw new InvalidOperationException("Invalid user first name."),
            User.FindFirst("middlename")?.Value ?? throw new InvalidOperationException("Invalid user middle name."),
            User.FindFirst(ClaimTypes.Surname)?.Value ?? throw new InvalidOperationException("Invalid user last name."),
            GetCompanyId());
    }

    public bool IsAllowed(string permission)
    {
        return User.IsAllowed(permissionsContainer, permission);
    }

    public IReadOnlyList<string> GetRoles()
    {
        return User
            .FindAll(ClaimTypes.Role)
            .Where(r => !string.IsNullOrEmpty(r.Value))
            .Select(x => x.Value)
            .ToList();
    }

    public IReadOnlyList<string> GetPermissions()
    {
        var claim = User.FindFirst(AuthConstants.PermissionClaimName)?.Value;
        if (string.IsNullOrEmpty(claim))
        {
            return Array.Empty<string>();
        }
        var permissionsBitArray = new BitArray(Convert.FromBase64String(claim));
        
        var permissions = new List<string>();
        foreach (var (permission, index) in permissionsContainer.Permissions)
        {
            if (permissionsBitArray.Length > index && permissionsBitArray[index])
            {
                permissions.Add(permission);
            }
        }
        
        return permissions;
    }
}
