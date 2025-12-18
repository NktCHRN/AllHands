using AllHands.Shared.Infrastructure.Auth;
using AllHands.Shared.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Marten;

namespace AllHands.NewsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuth();
        
        services.AddAllHandsMarten(configuration, _ =>
        {
            
        }).IntegrateWithWolverine();
        
        return services;
    }
}
