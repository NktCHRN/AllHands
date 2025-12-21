using AllHands.Shared.Domain.UserContext;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace AllHands.Shared.Infrastructure.UserContext;

public sealed class UserContextGrpcClientInterceptor(IUserContextAccessor userContextAccessor) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var userContext = userContextAccessor.UserContext;
        if (userContext is null)
        {
            return continuation(request, context);
        }
        
        var headers = context.Options.Headers ?? new Metadata();
        
        var userContextHeaders = UserContextGrpcMetadataProvider.GetMetadata(userContext);

        foreach (var entry in userContextHeaders)
        {
            RemoveAllHeaders(headers, entry.Key);
            headers.Add(entry);
        }

        var options = context.Options.WithHeaders(headers);
        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            options
        );

        return continuation(request, newContext);
    }
    
    private static void RemoveAllHeaders(Metadata headers, string key)
    {
        for (var i = headers.Count - 1; i >= 0; i--)
        {
            if (headers[i].Key == key)
            {
                headers.Remove(headers[i]);
            }
        }
    }
}
