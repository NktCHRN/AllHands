using MediatR;

namespace AllHands.AuthService.Application.Features.Employees.Update;

public sealed record UpdateEmployeeCommand(Guid UserId, 
    Guid PositionId,
    Guid ManagerId,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    DateOnly WorkStartDate) : 
    EmployeeCommandBase(PositionId, ManagerId, Email, FirstName, MiddleName, LastName, PhoneNumber, WorkStartDate), IRequest
{
    public Guid EmployeeId { get; set; }
}
