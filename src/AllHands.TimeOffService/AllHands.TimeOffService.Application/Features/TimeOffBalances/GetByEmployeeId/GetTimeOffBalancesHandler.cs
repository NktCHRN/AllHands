using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.GetByEmployeeId;

public sealed class GetTimeOffBalancesHandler(IQuerySession querySession) : IRequestHandler<GetTimeOffBalancesQuery, GetTimeOffBalancesResult>
{
    public async Task<GetTimeOffBalancesResult> Handle(GetTimeOffBalancesQuery request, CancellationToken cancellationToken)
    {
        var types = await querySession.Query<TimeOffType>()
            .OrderBy(b => b.Order)
            .ToListAsync(token: cancellationToken);
        
        var balances = await querySession.Query<TimeOffBalance>()
            .Where(b => b.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);

        var resultBalances = new List<TimeOffBalanceDto>();
        foreach (var type in types)
        {
            TimeOffBalanceDto dto = null!;
            var balance = balances.FirstOrDefault(b => b.TypeId == type.Id);
            if (balance is not null)
            {
                dto = new TimeOffBalanceDto(type.Id, type.Name, type.Emoji, Math.Round(balance.Days, 4), balance.DaysPerYear);
            }
            else
            {
                dto = new TimeOffBalanceDto(type.Id, type.Name, type.Emoji, 0, type.DaysPerYear);
            }
            resultBalances.Add(dto);
        }
        
        return new GetTimeOffBalancesResult(resultBalances);
    }
}
