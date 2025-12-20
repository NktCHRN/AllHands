using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.Get;

public sealed class GetTimeOffTypesHandler(IQuerySession querySession) : IRequestHandler<GetTimeOffTypesQuery, GetTimeOffTypesResult>
{
    public async Task<GetTimeOffTypesResult> Handle(GetTimeOffTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await querySession.Query<TimeOffType>()
            .OrderBy(t => t.Order)
            .ToListAsync(token: cancellationToken);

        return new GetTimeOffTypesResult(types
            .Select(t => new TimeOffTypeDto(t.Id, t.Order, t.Name, t.Emoji, t.DaysPerYear))
            .ToList());
    }
}
