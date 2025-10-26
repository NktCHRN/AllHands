namespace AllHands.Domain.Events.Employee;

public record EmployeeInfoUpdatedEvent(Guid EmployeeId, Guid PerformedByIdentityId, Guid PerformedByEmployeeId): BaseEmployeeEvent(EmployeeId, PerformedByIdentityId, PerformedByEmployeeId);