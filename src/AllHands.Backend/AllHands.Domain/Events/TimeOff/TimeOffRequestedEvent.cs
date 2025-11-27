using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.TimeOff;

public sealed record TimeOffRequestedEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    Guid EmployeeId,
    Guid CompanyId,
    Guid TypeId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal WorkingDaysCount,
    Guid TimeOffBalanceId) : AuditableEvent(EntityId, PerformedByUserId);
