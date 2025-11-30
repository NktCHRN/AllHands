using AllHands.Application.Dto;
using AllHands.Application.Queries;
using MediatR;

namespace AllHands.Application.Features.TimeOffBalances.GetMyHistory;

public sealed record GetMyTimeOffBalancesHistoryQuery(int PerPage, int Page) : PagedQuery(Page, PerPage), IRequest<PagedDto<TimeOffBalancesHistoryItemDto>>;
