using AllHands.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(IApplicationMarker).Assembly);
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssemblyContaining<IApplicationMarker>()
                .AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        return services.AddSingleton(TimeProvider.System);
    }
}
