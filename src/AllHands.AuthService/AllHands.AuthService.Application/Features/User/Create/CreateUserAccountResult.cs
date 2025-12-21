namespace AllHands.AuthService.Application.Features.User.Create;

public sealed record CreateUserAccountResult(Guid Id, Guid RoleId, Guid GlobalUserId);
