using AllHands.Shared.Application;
using AllHands.Shared.Application.Behaviors;
using AllHands.Shared.Domain;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.NewsService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddUserContext();
        services.AddBaseValidation();
        
        services.AddValidatorsFromAssembly(typeof(IApplicationMarker).Assembly);
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssemblyContaining<IApplicationMarker>()
                .AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        return services.AddSingleton(TimeProvider.System);
    }
}
