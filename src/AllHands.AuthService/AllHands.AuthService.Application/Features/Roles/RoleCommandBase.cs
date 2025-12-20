namespace AllHands.AuthService.Application.Features.Roles;

public abstract record RoleCommandBase(string Name, IReadOnlyList<string> Permissions);
