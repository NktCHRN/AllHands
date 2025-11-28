using System.Security.Claims;
using AllHands.Infrastructure.Auth.Entities;

namespace AllHands.Infrastructure.Auth;

public interface IUserClaimsFactory
{
    ClaimsPrincipal CreateClaimsPrincipal(AllHandsIdentityUser user);
    IReadOnlyList<Claim> CreateClaims(AllHandsIdentityUser user);
}