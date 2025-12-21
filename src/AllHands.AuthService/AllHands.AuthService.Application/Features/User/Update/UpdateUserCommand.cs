using MediatR;

namespace AllHands.AuthService.Application.Features.User.Update;

public sealed record UpdateUserCommand(Guid UserId, 
    Guid PositionId,
    Guid ManagerId,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    DateOnly WorkStartDate,
    Guid? RoleId) : 
    UserCommandBase(PositionId, ManagerId, Email, FirstName, MiddleName, LastName, PhoneNumber, WorkStartDate), IRequest
{
    public Guid EmployeeId { get; set; }
}
