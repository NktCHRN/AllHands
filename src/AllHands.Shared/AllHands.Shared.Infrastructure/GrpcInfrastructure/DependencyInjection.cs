using AllHands.Shared.Infrastructure.UserContext;
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
    
    public static IHttpClientBuilder AddInterceptors(
        this IHttpClientBuilder builder)
    {
        builder.Services.AddTransient<GrpcExceptionMappingInterceptor>();
        builder.Services.AddTransient<UserContextGrpcClientInterceptor>();

        builder.AddInterceptor<GrpcExceptionMappingInterceptor>();
        builder.AddInterceptor<UserContextGrpcClientInterceptor>();

        return builder;
    }
}
