using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssemblyContaining<IApplicationMarker>();
        });
        
        return services.AddSingleton(TimeProvider.System);
    }
}
