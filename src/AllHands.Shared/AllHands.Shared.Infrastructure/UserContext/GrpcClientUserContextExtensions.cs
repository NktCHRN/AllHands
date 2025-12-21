using Microsoft.Extensions.DependencyInjection;

namespace AllHands.Shared.Infrastructure.UserContext;

public static class GrpcClientUserContextExtensions
{
    public static IHttpClientBuilder AddUserContextInterceptor(
        this IHttpClientBuilder builder)
    {
        builder.Services.AddTransient<UserContextGrpcClientInterceptor>();

        builder.AddInterceptor<UserContextGrpcClientInterceptor>();

        return builder;
    }
}
