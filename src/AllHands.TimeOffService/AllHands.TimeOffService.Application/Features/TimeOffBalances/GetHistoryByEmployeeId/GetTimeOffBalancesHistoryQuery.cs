using AllHands.Shared.Application.Dto;
using AllHands.Shared.Application.Queries;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;

public sealed record GetTimeOffBalancesHistoryQuery(Guid EmployeeId, int PerPage, int Page) : PagedQuery(PerPage, Page), IRequest<PagedDto<TimeOffBalancesHistoryItemDto>>;
