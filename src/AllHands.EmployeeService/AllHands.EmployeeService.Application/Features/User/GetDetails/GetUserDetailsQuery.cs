using AllHands.EmployeeService.Application.Dto;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.GetDetails;

public sealed record GetUserDetailsQuery() : IRequest<EmployeeDetailsDto>;
