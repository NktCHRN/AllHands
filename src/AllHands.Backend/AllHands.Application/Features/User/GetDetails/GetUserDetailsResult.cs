using AllHands.Application.Features.User.Dto;

namespace AllHands.Application.Features.User.GetDetails;

public sealed record GetUserDetailsResult(
    Guid EmployeeId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    string? PhoneNumber,
    EmployeeDto Manager,
    PositionDto Position,
    CompanyDto Company);
    