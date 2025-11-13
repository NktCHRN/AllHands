using System.Security.Claims;

namespace AllHands.Application.Abstractions;

public sealed record LoginResult(bool IsSuccessful, ClaimsPrincipal? ClaimsPrincipal);
