namespace AllHands.Auth.Contracts.Rest;

public sealed record UserContextResponse(
    string Id,
    string Email,
    string? PhoneNumber,
    string CompanyId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string EmployeeId,
    IReadOnlyList<string> Roles,
    string Permissions);
    