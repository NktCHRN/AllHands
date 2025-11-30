using AllHands.Application.Abstractions;
using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffTypes.Update;

public sealed class UpdateTimeOffTypeHandler(IDocumentStore documentStore, ICurrentUserService currentUserService, TimeProvider timeProvider) : IRequestHandler<UpdateTimeOffTypeCommand>
{
    public async Task Handle(UpdateTimeOffTypeCommand request, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();
        await using var session = await documentStore.LightweightSerializableSessionAsync(companyId.ToString(), cancellationToken);
        
        var types = await session.Query<TimeOffType>()
            .ToListAsync(token: cancellationToken);
        var requestedType = types.FirstOrDefault(t => t.Id == request.Id)
                   ?? throw new EntityNotFoundException("Time off type was not found.");
        
        var minOrder = types.Min(t => t.Order);
        var maxOrder = types.Max(t => t.Order);
        if (request.Order < minOrder || request.Order > maxOrder)
        {
            throw new EntityValidationFailedException("Order must be between " + minOrder + " and " + maxOrder);
        }
        
        requestedType.Name = request.Name;
        requestedType.Emoji = request.Emoji;
        var delta = request.DaysPerYear - requestedType.DaysPerYear;
        requestedType.DaysPerYear = request.DaysPerYear;
        var oldOrder = requestedType.Order;
        var newOrder = request.Order;
        requestedType.Order = newOrder;
        session.Update(requestedType);

        foreach (var timeOffType in types)
        {
            if (timeOffType.Id == requestedType.Id)
            {
                continue;
            }
            
            if (newOrder > oldOrder && timeOffType.Order > oldOrder && timeOffType.Order <= newOrder)
            {
                timeOffType.Order--;
                session.Update(timeOffType);
            }

            if (newOrder < oldOrder && timeOffType.Order >= newOrder && timeOffType.Order < oldOrder)
            {
                timeOffType.Order++;
                session.Update(timeOffType);
            }
        }

        if (delta != 0)
        {
            if (request.TimeOffUpdateType is null or TimeOffPerYearUpdateType.Undefined)
            {
                throw new EntityValidationFailedException("TimeOffUpdateType must be specified if you want to update days per year.");
            }

            var userId = currentUserService.GetId();
            await foreach (var balance in session.Query<TimeOffBalance>().Where(b => b.TypeId == request.Id)
                               .ToAsyncEnumerable(token: cancellationToken))
            {
                session.Events.Append(balance.Id, new TimeOffBalancePerYearUpdatedEvent(balance.Id, userId, request.DaysPerYear, delta, request.TimeOffUpdateType.Value));
            }
        }
        
        await session.SaveChangesAsync(cancellationToken);
    }
}
