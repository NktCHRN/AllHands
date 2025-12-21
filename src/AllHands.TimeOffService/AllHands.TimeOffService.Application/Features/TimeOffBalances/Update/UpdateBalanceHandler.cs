using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.TimeOffService.Domain.Events.TimeOffBalance;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.Update;

public sealed class UpdateBalanceHandler(IDocumentSession session, IUserContext userContext) : IRequestHandler<UpdateBalanceCommand>
{
    public async Task Handle(UpdateBalanceCommand request, CancellationToken cancellationToken)
    {
        var timeOffTypeExists = await session.Query<TimeOffType>()
            .AnyAsync(t => t.Id == request.TypeId, cancellationToken);

        if (!timeOffTypeExists)
        {
            throw new EntityNotFoundException("Time off type was not found.");
        }

        var balance = await session.Query<TimeOffBalance>()
            .FirstOrDefaultAsync(b => b.EmployeeId == request.EmployeeId && b.TypeId == request.TypeId,
                cancellationToken);

        if (balance == null)
        {
            var balanceId = TimeOffBalance.CreateId(request.EmployeeId, request.TypeId);
            var events = new List<object>
            {
                new TimeOffBalanceCreatedEvent(balanceId, request.EmployeeId, request.TypeId, request.DaysPerYear.GetValueOrDefault())
            };
            if (request.Delta != 0)
            {
                events.Add(new TimeOffBalanceManuallyUpdated(balanceId, userContext.Id, userContext.EmployeeId, request.Reason, request.Delta));
            }
            session.Events.StartStream<TimeOffBalance>(balanceId, events);
            await session.SaveChangesAsync(cancellationToken);
            return;
        }

        if (request.DaysPerYear.HasValue && request.DaysPerYear != balance.DaysPerYear)
        {
            var perYearDelta = request.DaysPerYear.Value - balance.DaysPerYear;
            session.Events.Append(balance.Id,
                new TimeOffBalancePerYearUpdatedEvent(balance.Id, userContext.Id, request.DaysPerYear,
                    perYearDelta, TimeOffPerYearUpdateType.Reset));
        }

        if (request.Delta != 0)
        {
            session.Events.Append(balance.Id, new TimeOffBalanceManuallyUpdated(balance.Id, userContext.Id, userContext.EmployeeId, request.Reason, request.Delta));
        }
        
        await session.SaveChangesAsync(cancellationToken);
    }
}
