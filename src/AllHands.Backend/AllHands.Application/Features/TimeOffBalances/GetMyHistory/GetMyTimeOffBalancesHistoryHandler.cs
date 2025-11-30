using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffBalances.GetMyHistory;

public sealed class GetMyTimeOffBalancesHistoryHandler(IQuerySession querySession, ICurrentUserService currentUserService) : IRequestHandler<GetMyTimeOffBalancesHistoryQuery, PagedDto<TimeOffBalancesHistoryItemDto>>
{
    public Task<PagedDto<TimeOffBalancesHistoryItemDto>> Handle(GetMyTimeOffBalancesHistoryQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
