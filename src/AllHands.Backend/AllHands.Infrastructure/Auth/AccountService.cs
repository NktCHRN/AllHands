using System.Security.Claims;
using AllHands.Application.Abstractions;
using AllHands.Infrastructure.Auth.Entities;
using DomainAbstractions.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Auth;

public sealed class AccountService(UserManager<AllHandsIdentityUser> userManager, RoleManager<IdentityRole<Guid>> roleManager) : IAccountService
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
        var roles = await userManager.GetRolesAsync(user);
        throw new NotImplementedException();
    }
}
