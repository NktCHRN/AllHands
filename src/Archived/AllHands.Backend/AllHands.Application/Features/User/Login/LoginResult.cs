using System.Security.Claims;

namespace AllHands.Application.Features.User.Login;

public sealed record LoginResult(ClaimsPrincipal ClaimsPrincipal);
