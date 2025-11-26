using AllHands.Domain.Abstractions;
using AllHands.Domain.Events.TimeOff;
using AllHands.Domain.Models;
using JasperFx.Events;
using JasperFx.Events.Grouping;
using Marten;
using Marten.Events.Aggregation;

namespace AllHands.Domain.EventGroupers;

public class TimeOffBalanceTimeOffRequestCancelledRejectedEventGrouper : IAggregateGrouper<Guid>
{
    private readonly HashSet<Type> _eventTypes = new HashSet<Type> { typeof(IEvent<TimeOffRequestCancelledEvent>), typeof(IEvent<TimeOffRequestCancelledEvent>) };
    
    public async Task Group(IQuerySession session, IEnumerable<IEvent> events, IEventGrouping<Guid> grouping)
    {
        var timeOffCancelledEvents = events
            .Where(e => _eventTypes.Contains(e.GetType()))
            .Select(e => (IEvent<AuditableEvent>)e)
            .ToList();
        
        if (timeOffCancelledEvents.Count == 0)
        {
            return;
        }

        var timeOffRequestsIds = timeOffCancelledEvents
            .Select(x => x.Data.EntityId)
            .ToList();
        var timeOffRequests = await session.Query<TimeOffRequest>()
            .Where(x => timeOffRequestsIds.Contains(x.EmployeeId))
            .ToListAsync();
        
        var identifiers = timeOffRequests
            .Select(r => (r.EmployeeId, r.TypeId))
            .Distinct()
            .ToList();
        var employeeIds = identifiers
            .Select(i => i.EmployeeId)
            .Distinct()
            .ToArray();
        var typeIds = identifiers
            .Select(i => i.TypeId)
            .Distinct()
            .ToArray();
        var employeeBalanceItems = await session.Query<TimeOffBalance>()
            .Where(x => employeeIds.Contains(x.EmployeeId) && typeIds.Contains(x.TypeId))
            .ToListAsync();
        
        var streamIds = new Dictionary<Guid, Guid>();
        foreach (var @event in timeOffCancelledEvents)
        {
            var timeOffRequest = timeOffRequests.First(r => r.Id == @event.Data.EntityId);
            var balanceItem = employeeBalanceItems.First(e => e.EmployeeId == timeOffRequest.EmployeeId && e.TypeId == timeOffRequest.TypeId);
            streamIds[@event.Data.EntityId] = balanceItem.Id;

        }
        
        grouping.AddEvents<TimeOffRequestCancelledEvent>(e => streamIds[e.EntityId], timeOffCancelledEvents);
    }
}
