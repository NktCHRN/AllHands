using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.GetByEmployeeId;

public sealed record GetTimeOffBalancesQuery(Guid EmployeeId) : IRequest<GetTimeOffBalancesResult>;
