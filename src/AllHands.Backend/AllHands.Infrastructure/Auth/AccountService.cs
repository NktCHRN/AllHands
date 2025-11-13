using System.Security.Claims;
using AllHands.Application.Abstractions;
using AllHands.Infrastructure.Auth.Entities;
using DomainAbstractions.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Auth;

public sealed class AccountService(UserManager<AllHandsIdentityUser> userManager, AuthDbContext dbContext) : IAccountService
{
    public async Task<LoginResult> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(login);
        var checkPasswordResult = user is not null && await userManager.CheckPasswordAsync(user, password);
        if (!checkPasswordResult)
        {
            throw new UserUnauthorizedException("Either login or password is incorrect.");
        }
        
        throw new NotImplementedException();
    }

    private async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(AllHandsIdentityUser user)
    {
        // TODO: Implement navigation. User => UserRole => Role => RoleClaim => Claim.
        throw new NotImplementedException();
        // var roleNames = await userManager.GetRolesAsync(user);
        // var permissions = new List<Claim>();
        // foreach (var roleName in roleNames)
        // {
        //     var role = await roleManager.FindByNameAsync(roleName)
        //         ;
        //     permissions.AddRange(await roleManager.GetClaimsAsync(role));
        // }
    }
}
