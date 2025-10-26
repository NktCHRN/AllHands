namespace AllHands.Domain.Events.Employee;

public record EmployeeRehiredEvent(Guid EmployeeId, Guid PerformedByIdentityId, Guid PerformedByEmployeeId): BaseEmployeeEvent(EmployeeId, PerformedByIdentityId, PerformedByEmployeeId);  // Fired employee could be rehired.
