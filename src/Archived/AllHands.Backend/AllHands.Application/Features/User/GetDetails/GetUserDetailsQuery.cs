using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.User.GetDetails;

public sealed record GetUserDetailsQuery() : IRequest<EmployeeDetailsDto>;
