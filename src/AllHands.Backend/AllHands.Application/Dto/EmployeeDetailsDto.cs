using AllHands.Domain.Models;

namespace AllHands.Application.Dto;

public sealed record EmployeeDetailsDto(
    Guid EmployeeId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    string? PhoneNumber,
    EmployeeStatus Status,
    DateOnly WorkStartDate,
    EmployeeDto Manager,
    PositionDto Position,
    CompanyDto Company,
    RoleDto Role);
    