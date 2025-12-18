namespace AllHands.Shared.Application.Auth;

public interface IUserPermissionService
{
    bool IsAllowed(string permission);
    IReadOnlyList<string> GetPermissions();
}
