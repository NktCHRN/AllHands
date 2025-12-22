using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.Register;

public sealed record RegisterUserCommand(Guid UserId) : IRequest;
