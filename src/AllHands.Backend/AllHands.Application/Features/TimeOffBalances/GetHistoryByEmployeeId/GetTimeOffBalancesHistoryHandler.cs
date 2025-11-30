using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;

public sealed class GetTimeOffBalancesHistoryHandler(IQuerySession querySession, ICurrentUserService currentUserService) : IRequestHandler<GetTimeOffBalancesHistoryQuery, PagedDto<TimeOffBalancesHistoryItemDto>>
{
    public async Task<PagedDto<TimeOffBalancesHistoryItemDto>> Handle(GetTimeOffBalancesHistoryQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
