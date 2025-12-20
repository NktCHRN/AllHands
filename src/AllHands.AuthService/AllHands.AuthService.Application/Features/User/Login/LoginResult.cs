using System.Security.Claims;

namespace AllHands.AuthService.Application.Features.User.Login;

public sealed record LoginResult(ClaimsPrincipal ClaimsPrincipal);
