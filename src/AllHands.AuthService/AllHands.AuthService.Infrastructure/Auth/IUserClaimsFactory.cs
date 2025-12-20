using System.Security.Claims;
using AllHands.AuthService.Domain.Models;

namespace AllHands.AuthService.Infrastructure.Auth;

public interface IUserClaimsFactory
{
    ClaimsPrincipal CreateClaimsPrincipal(AllHandsIdentityUser user);
    IReadOnlyList<Claim> CreateClaims(AllHandsIdentityUser user);
}