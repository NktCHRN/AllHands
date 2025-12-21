namespace AllHands.AuthService.Application.Features.User.Update;

public sealed record UpdateUserResult(Guid Id, Guid RoleId, Guid GlobalUserId);
