using AllHands.Shared.Application.Abstractions;
using AllHands.Shared.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.Shared.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBaseValidation(this IServiceCollection services)
    {
        services.AddSingleton<IImageValidator, ImageValidator>();
        return services.AddValidatorsFromAssembly(typeof(IMarker).Assembly);
    }
}
