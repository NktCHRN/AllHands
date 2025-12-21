using AllHands.Shared.Domain.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace AllHands.Shared.Infrastructure.GrpcInfrastructure;

public sealed class GrpcExceptionMappingInterceptor : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return continuation(request, context);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            throw new EntityNotFoundException(ex.Status.Detail);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.PermissionDenied)
        {
            throw new ForbiddenForUserException(ex.Status.Detail);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
        {
            throw new EntityAlreadyExistsException(ex.Status.Detail);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            throw new EntityValidationFailedException(ex.Status.Detail);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
        {
            throw new UserUnauthorizedException(ex.Status.Detail);
        }
    }
}
