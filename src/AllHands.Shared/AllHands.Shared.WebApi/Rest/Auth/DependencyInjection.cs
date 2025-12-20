using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.Shared.WebApi.Rest.Auth;

public static class DependencyInjection
{
    public static AuthenticationBuilder AddPermissionBasedAuth(this IServiceCollection services)
    {
        var result = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ServiceAuthHandler.SchemeName;
                options.DefaultChallengeScheme = ServiceAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, ServiceAuthHandler>(ServiceAuthHandler.SchemeName, _ =>
            {
            });

        services.AddAuthorization();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        
        return result;
    }
}
