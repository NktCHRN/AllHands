namespace AllHands.Application.Features.Employee.Create;

public sealed record SendCompleteRegistrationEmailCommand(string Email, string FirstName, string AdminName, Guid InvitationId, string Token);
