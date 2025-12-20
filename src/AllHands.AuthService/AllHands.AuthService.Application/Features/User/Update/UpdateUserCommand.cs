using MediatR;

namespace AllHands.AuthService.Application.Features.User.Update;

public sealed record UpdateUserCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber) : IRequest;
