using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public sealed record EmployeeFiredEvent(
    Guid EntityId, 
    Guid PerformedByUserId,
    string Reason): AuditableEvent(EntityId, PerformedByUserId);
    