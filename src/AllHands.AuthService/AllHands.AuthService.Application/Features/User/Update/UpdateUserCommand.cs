using MediatR;

namespace AllHands.AuthService.Application.Features.User.Update;

public sealed record UpdateUserCommand(
    Guid UserId,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    Guid? RoleId) :
    UserCommandBase(Email, FirstName, MiddleName, LastName, PhoneNumber), IRequest;
