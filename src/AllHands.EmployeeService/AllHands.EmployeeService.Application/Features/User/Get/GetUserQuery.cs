using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.Get;

public sealed record GetUserQuery() : IRequest<GetUserResult>;
