using Grpc.AspNetCore.Server;

namespace AllHands.Shared.WebApi.Grpc;

public static class GrpcExtensions
{
    public static void AddsInterceptors(this GrpcServiceOptions options)
    {
        options.Interceptors.Add<ExceptionHandlingInterceptor>();
        options.Interceptors.Add<UserContextHeadersInterceptor>();
    }
}
