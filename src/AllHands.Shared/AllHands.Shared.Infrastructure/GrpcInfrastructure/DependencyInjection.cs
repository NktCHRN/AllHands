using Microsoft.Extensions.DependencyInjection;

namespace AllHands.Shared.Infrastructure.GrpcInfrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGrpcRetryOptions(this IServiceCollection services, string configurationSectionPath = "GrpcRetryOptions")
    {
        services.AddOptions<GrpcRetryOptions>()
            .BindConfiguration(configurationSectionPath);
        
        return services;
    }
}
