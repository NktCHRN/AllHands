using AllHands.Shared.Domain.Abstractions;

namespace AllHands.TimeOffService.Domain.Events.TimeOff;

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
