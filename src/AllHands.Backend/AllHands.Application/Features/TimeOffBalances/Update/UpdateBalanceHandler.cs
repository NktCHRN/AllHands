using AllHands.Application.Abstractions;
using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffBalances.Update;

public sealed class UpdateBalanceHandler(IDocumentSession session, ICurrentUserService currentUserService) : IRequestHandler<UpdateBalanceCommand>
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
                events.Add(new TimeOffBalanceManuallyUpdated(balanceId, currentUserService.GetId(), currentUserService.GetEmployeeId(), request.Reason, request.Delta));
            }
            session.Events.StartStream(balanceId, events);
            await session.SaveChangesAsync(cancellationToken);
            return;
        }

        if (request.DaysPerYear.HasValue && request.DaysPerYear != balance.DaysPerYear)
        {
            var perYearDelta = request.DaysPerYear.Value - balance.DaysPerYear;
            session.Events.Append(balance.Id,
                new TimeOffBalancePerYearUpdatedEvent(balance.Id, currentUserService.GetId(), request.DaysPerYear,
                    perYearDelta, TimeOffPerYearUpdateType.Reset));
        }

        if (request.Delta != 0)
        {
            session.Events.Append(balance.Id, new TimeOffBalanceManuallyUpdated(balance.Id, currentUserService.GetId(), currentUserService.GetEmployeeId(), request.Reason, request.Delta));
        }
        
        await session.SaveChangesAsync(cancellationToken);
    }
}
