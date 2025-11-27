namespace AllHands.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalanceCreatedEvent(Guid EntityId, Guid EmployeeId, Guid TypeId);
