using AllHands.Shared.Domain.UserContext;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.Shared.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddSingleton<UserContextService>();
        services.AddSingleton<IUserContextAccessor>(sp => sp.GetRequiredService<UserContextService>());
        services.AddSingleton<IUserContextSetuper>(sp => sp.GetRequiredService<UserContextService>());
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<IUserContextAccessor>().UserContext ?? throw new InvalidOperationException("No user context provided."));
        
        return services;
    }
}
