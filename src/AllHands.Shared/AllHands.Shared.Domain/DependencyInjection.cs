using AllHands.Shared.Domain.UserContext;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.Shared.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddScoped<UserContext.UserContext>();
        services.AddSingleton<IUserContextAccessor, UserContextAccessor>();
        
        return services;
    }
}
