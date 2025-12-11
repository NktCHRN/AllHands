using DeterministicGuids;

namespace AllHands.TimeOffBalanceAutoUpdater.Models;

public sealed class TimeOffBalance
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid TypeId { get; set; }
    public decimal Days { get; set; }
    public decimal DaysPerYear { get; set; }

    public DateTimeOffset? LastAutoUpdate { get; set; }
    
    public static Guid CreateId(Guid employeeId, Guid typeId) => DeterministicGuid.Create(employeeId, typeId.ToString()); 
}
