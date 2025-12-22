using System.Security.Claims;
using AllHands.Infrastructure.Auth.Entities;

namespace AllHands.Infrastructure.Auth;

public interface IUserClaimsFactory
{
    ClaimsPrincipal CreateClaimsPrincipal(AllHandsIdentityUser user, Guid employeeId);
    IReadOnlyList<Claim> CreateClaims(AllHandsIdentityUser user, Guid employeeId);
}