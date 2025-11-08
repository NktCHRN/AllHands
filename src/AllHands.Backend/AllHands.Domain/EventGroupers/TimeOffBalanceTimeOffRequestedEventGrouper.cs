using AllHands.Domain.Events.TimeOff;
using AllHands.Domain.Models;
using DeterministicGuids;
using JasperFx.Events;
using JasperFx.Events.Grouping;
using Marten;
using Marten.Events.Aggregation;

namespace AllHands.Domain.EventGroupers;

public class TimeOffBalanceTimeOffRequestedEventGrouper : IAggregateGrouper<Guid>
{
    public async Task Group(IQuerySession session, IEnumerable<IEvent> events, IEventGrouping<Guid> grouping)
    {
        var eventsList = events.ToList();
        var timeOffRequestedEvents = eventsList
            .OfType<IEvent<TimeOffRequestedEvent>>()
            .ToList();

        var identifiers = timeOffRequestedEvents
            .Select(r => (r.Data.EmployeeId, r.Data.TypeId))
            .Distinct()
            .ToList();
        var employeeBalanceItems = await session.Query<TimeOffBalance>()
            .Where(x => identifiers.Contains(new(x.EmployeeId, x.TypeId)))
            .ToListAsync<TimeOffBalance>();
        
        var streamIds = new Dictionary<(Guid, Guid), Guid>();
        foreach (var id in identifiers)
        {
            streamIds[id] =
                employeeBalanceItems.FirstOrDefault(e => e.EmployeeId == id.EmployeeId && e.TypeId == id.TypeId)?.Id
                ?? DeterministicGuid.Create(id.EmployeeId, id.TypeId.ToString());
        }
        
        grouping.AddEvents<TimeOffRequestedEvent>(e => streamIds[(e.EmployeeId, e.TypeId)], timeOffRequestedEvents);
    }
}
