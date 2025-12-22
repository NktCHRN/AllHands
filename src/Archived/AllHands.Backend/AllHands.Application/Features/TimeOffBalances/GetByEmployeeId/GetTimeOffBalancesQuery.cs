using MediatR;

namespace AllHands.Application.Features.TimeOffBalances.GetByEmployeeId;

public sealed record GetTimeOffBalancesQuery(Guid EmployeeId) : IRequest<GetTimeOffBalancesResult>;
