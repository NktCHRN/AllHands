namespace AllHands.Domain.Events.Employee;

public record EmployeeDeletedEvent(Guid EmployeeId, Guid PerformedByIdentityId, Guid PerformedByEmployeeId): BaseEmployeeEvent(EmployeeId, PerformedByIdentityId, PerformedByEmployeeId);