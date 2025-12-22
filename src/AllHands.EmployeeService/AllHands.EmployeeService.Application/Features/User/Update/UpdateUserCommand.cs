using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.Update;

public sealed record UpdateUserCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber) : IRequest;
