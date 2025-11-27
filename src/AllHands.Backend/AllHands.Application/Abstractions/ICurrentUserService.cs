namespace AllHands.Application.Abstractions;

public interface ICurrentUserService
{
    Guid GetId();
    string GetEmail();
    string? GetPhoneNumber();
    CurrentUserDto GetCurrentUser();
    bool IsAllowed(string permission);
    string GetCompanyId();
}
