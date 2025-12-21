using AllHands.Shared.Domain.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace AllHands.Shared.WebApi.Grpc;

public sealed class ExceptionHandlingInterceptor(ILogger<ExceptionHandlingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            var baseException = ex.GetBaseException();
            
            var (statusCode, message) = baseException switch
            {
                EntityAlreadyExistsException => (StatusCode.AlreadyExists, baseException.Message),
                EntityNotFoundException => (StatusCode.NotFound, baseException.Message),
                EntityValidationFailedException => (StatusCode.InvalidArgument, baseException.Message),
                UserUnauthorizedException => (StatusCode.Unauthenticated, baseException.Message),
                ForbiddenForUserException => (StatusCode.PermissionDenied, baseException.Message),
                _ => (StatusCode.Internal, "An unexpected error occured on the server.")
            };
            
            if (statusCode is StatusCode.Internal)
            {
                logger.LogError("An unexpected error occured: {ex}", ex);
            }
            
            throw new RpcException(
                new Status(statusCode, message), message);
        }
    }
}
