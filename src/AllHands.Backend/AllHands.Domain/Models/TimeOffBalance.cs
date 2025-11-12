using AllHands.Domain.EventGroupers;
using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Events.TimeOff;
using DeterministicGuids;
using JasperFx.Events;
using Marten.Events.Projections;

namespace AllHands.Domain.Models;

public sealed class TimeOffBalance
{
    public Guid Id { get; internal set; }
    public Guid EmployeeId { get; internal set; }
    public Guid TypeId { get; internal set; }
    public TimeOffType? Type { get; internal set; }
    public decimal Days { get; internal set; }

    public static Guid GetId(Guid employeeId, Guid typeId) =>
        DeterministicGuid.Create(employeeId, typeId.ToString());
}
