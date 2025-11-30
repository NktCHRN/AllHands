using AllHands.Application.Dto;
using AllHands.Application.Queries;
using MediatR;

namespace AllHands.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;

public sealed record GetTimeOffBalancesHistoryQuery(Guid EmployeeId, int PerPage, int Page) : PagedQuery(Page, PerPage), IRequest<PagedDto<TimeOffBalancesHistoryItemDto>>;
