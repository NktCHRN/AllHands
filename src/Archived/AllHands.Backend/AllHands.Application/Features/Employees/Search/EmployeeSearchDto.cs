using AllHands.Application.Dto;
using AllHands.Domain.Models;

namespace AllHands.Application.Features.Employees.Search;

public sealed record EmployeeSearchDto(Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    string? PhoneNumber,
    EmployeeStatus Status,
    PositionDto? Position);
    