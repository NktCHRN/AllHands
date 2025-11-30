using AllHands.Application.Dto;

namespace AllHands.Application.Features.User.GetDetails;

public sealed record GetUserDetailsResult(
    Guid EmployeeId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateOnly WorkStartDate,
    EmployeeDto Manager,
    PositionDto Position,
    CompanyDto Company,
    RoleDto Role);
    