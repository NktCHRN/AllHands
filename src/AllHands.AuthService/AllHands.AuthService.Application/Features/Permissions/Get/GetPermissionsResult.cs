namespace AllHands.AuthService.Application.Features.Permissions.Get;

public sealed record GetPermissionsResult(IReadOnlyList<string> Permissions);
