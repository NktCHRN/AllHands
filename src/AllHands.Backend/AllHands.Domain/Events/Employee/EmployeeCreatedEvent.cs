namespace AllHands.Domain.Events.Employee;

public record EmployeeCreatedEvent(Guid EmployeeId, Guid PerformedByIdentityId, Guid PerformedByEmployeeId): BaseEmployeeEvent(EmployeeId, PerformedByIdentityId, PerformedByEmployeeId);
