using AllHands.Shared.Application.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AllHands.Shared.Infrastructure.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.TryAddScoped<Domain.UserContext.UserContext>();
        
        services.AddSingleton<IPermissionsContainer, PermissionsContainer>();
        services.AddSingleton<IUserPermissionService, UserPermissionService>();
        
        return services;
    }
}
