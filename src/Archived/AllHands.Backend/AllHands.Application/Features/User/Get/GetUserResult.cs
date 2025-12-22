using AllHands.Application.Dto;

namespace AllHands.Application.Features.User.Get;

public sealed record GetUserResult(
    Guid EmployeeId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    string? PhoneNumber,
    PositionDto Position,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
