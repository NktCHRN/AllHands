using Grpc.AspNetCore.Server;

namespace AllHands.Shared.WebApi.Grpc;

public static class GrpcExtensions
{
    public static void AddUserContextHeadersInterceptor(this GrpcServiceOptions options)
    {
        options.Interceptors.Add<UserContextHeadersInterceptor>();
    }
}
