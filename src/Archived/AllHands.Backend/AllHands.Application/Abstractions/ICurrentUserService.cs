using AllHands.Application.Dto;

namespace AllHands.Application.Abstractions;

public interface ICurrentUserService
{
    Guid GetId();
    string GetEmail();
    string? GetPhoneNumber();
    CurrentUserDto GetCurrentUser();
    bool IsAllowed(string permission);
    Guid GetCompanyId();
    IReadOnlyList<string> GetRoles();
    IReadOnlyList<string> GetPermissions();
    bool TryGetCompanyId(out Guid companyId);
    Guid GetEmployeeId();
}
