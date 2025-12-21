namespace AllHands.AuthService.Application.Features.User.Create;

public sealed record SendCompleteRegistrationEmailCommand(
    string Email,
    string FirstName,
    string AdminName,
    Guid InvitationId,
    string Token);
    