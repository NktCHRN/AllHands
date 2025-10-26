using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.Employee;

public abstract record BaseEmployeeEvent(Guid EmployeeId, Guid PerformedByIdentityId, Guid PerformedByEmployeeId) : AuditableEvent(PerformedByIdentityId, PerformedByEmployeeId);