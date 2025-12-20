using AllHands.TimeOffService.Application.Dto;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.Holiday.Get;

public sealed class GetHolidaysHandler(IQuerySession querySession) : IRequestHandler<GetHolidaysQuery, GetHolidaysResult>
{
    public async Task<GetHolidaysResult> Handle(GetHolidaysQuery request, CancellationToken cancellationToken)
    {
        var holidays = await querySession.Query<Domain.Models.Holiday>()
            .Where(h => h.Date >= request.Start && h.Date <= request.End)
            .OrderBy(x => x.Date)
            .ToListAsync(token: cancellationToken);

        return new GetHolidaysResult(holidays
            .Select(h => new HolidayDto(h.Id, h.Name, h.Date))
            .ToList());
    }
}
