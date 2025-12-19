using System.Collections;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Infrastructure.UserContext;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace AllHands.Shared.WebApi.Grpc;

public sealed class UserContextHeadersInterceptor(IUserContextSetuper userContextSetuper) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        // Read headers
        var headers = context.RequestHeaders;

        var userContext = new UserContext();
        
        if (!string.IsNullOrEmpty(headers.GetValue(UserContextHeaders.Id)))
        {
            userContext.Id = Guid.Parse(headers.GetValue(UserContextHeaders.Id)!);
        }
        
        userContext.Email = headers.GetValue(UserContextHeaders.Email) ?? string.Empty;
        
        userContext.PhoneNumber = headers.GetValue(UserContextHeaders.PhoneNumber);
        
        if (!string.IsNullOrEmpty(headers.GetValue(UserContextHeaders.CompanyId)))
        {
            userContext.CompanyId = Guid.Parse(headers.GetValue(UserContextHeaders.CompanyId)!);
        }
        
        if (!string.IsNullOrEmpty(headers.GetValue(UserContextHeaders.EmployeeId)))
        {
            userContext.EmployeeId = Guid.Parse(headers.GetValue(UserContextHeaders.EmployeeId)!);
        }

        userContext.FirstName = headers.GetValue(UserContextHeaders.FirstName) ?? string.Empty;

        userContext.MiddleName = headers.GetValue(UserContextHeaders.MiddleName);

        userContext.LastName = headers.GetValue(UserContextHeaders.LastName) ?? string.Empty;

        userContext.Roles = headers
            .Where(h => h.Key == UserContextHeaders.Roles)
            .Select(h => h.Value)
            .ToList();

        var permissionsByteArray = headers.GetValueBytes(UserContextHeaders.PermissionsBin);
        if (permissionsByteArray is not null)
        {
            userContext.Permissions = new BitArray(permissionsByteArray);
        }

        userContextSetuper.Push(userContext);
        
        return await continuation(request, context);
    }
}
