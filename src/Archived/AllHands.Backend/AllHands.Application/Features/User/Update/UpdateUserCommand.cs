using MediatR;

namespace AllHands.Application.Features.User.Update;

public sealed record UpdateUserCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber) : IRequest;
