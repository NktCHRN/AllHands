namespace AllHands.AuthService.Application.Features.Roles.GetById;

public sealed record GetRoleByIdResult(Guid Id, string Name, bool IsDefault, int UsersCount, IReadOnlyCollection<string> Permissions);
