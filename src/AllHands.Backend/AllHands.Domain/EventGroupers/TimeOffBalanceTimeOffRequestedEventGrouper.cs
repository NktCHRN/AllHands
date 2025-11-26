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
        
        // (EmployeeId, TypeId) combinations from events
        var identifiers = timeOffRequestedEvents
            .Select(r => (r.Data.EmployeeId, r.Data.TypeId))
            .Distinct()
            .ToArray();
        
        var employeeIds = identifiers
            .Select(i => i.EmployeeId)
            .Distinct()
            .ToArray();

        var typeIds = identifiers
            .Select(i => i.TypeId)
            .Distinct()
            .ToArray();
        
        var employeeBalanceItems = await session.Query<TimeOffBalance>()
            .Where(x => employeeIds.Contains(x.EmployeeId) &&
                        typeIds.Contains(x.TypeId))
            .ToListAsync();
        
        var balancesByKey = employeeBalanceItems
            .GroupBy(b => (b.EmployeeId, b.TypeId))
            .ToDictionary(g => g.Key, g => g.First().Id);
        
        var streamIds = new Dictionary<(Guid EmployeeId, Guid TypeId), Guid>();

        foreach (var id in identifiers)
        {
            if (balancesByKey.TryGetValue(id, out var existingId))
            {
                streamIds[id] = existingId;
            }
            else
            {
                streamIds[id] = DeterministicGuid.Create(id.EmployeeId, id.TypeId.ToString());
            }
        }
        
        grouping.AddEvents<TimeOffRequestedEvent>(e => streamIds[(e.EmployeeId, e.TypeId)], timeOffRequestedEvents);
    }
}
