namespace AllHands.Domain.Events.Employee;

public record EmployeeFiredEvent(Guid EmployeeId, Guid PerformedByIdentityId, Guid PerformedByEmployeeId): BaseEmployeeEvent(EmployeeId, PerformedByIdentityId, PerformedByEmployeeId);