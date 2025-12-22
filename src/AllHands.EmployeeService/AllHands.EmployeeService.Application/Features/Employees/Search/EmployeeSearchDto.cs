using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Models;

namespace AllHands.EmployeeService.Application.Features.Employees.Search;

public sealed record EmployeeSearchDto(Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    string? PhoneNumber,
    EmployeeStatus Status,
    PositionDto? Position);
    