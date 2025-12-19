using System.Collections;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Infrastructure.UserContext;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AllHands.Shared.WebApi.Rest;

public sealed class UserContextHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IUserContextSetuper _userContextSetuper;
    
    public UserContextHeadersMiddleware(RequestDelegate next, IUserContextSetuper userContextSetuper)
    {
        _userContextSetuper = userContextSetuper;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var headers = httpContext.Request.Headers;
        
        if (IsGrpcRequest(httpContext.Request))
        {
            await _next(httpContext);
            return;
        }

        var userContext = new UserContext();
        
        if (headers.TryGetValue(UserContextHeaders.Id, out var idValues) &&
            Guid.TryParse(idValues.ToString(), out var userId))
        {
            userContext.Id = userId;
        }

        if (headers.TryGetValue(UserContextHeaders.Email, out var emailValues))
        {
            userContext.Email = emailValues.ToString();
        }

        if (headers.TryGetValue(UserContextHeaders.PhoneNumber, out var phoneValues))
        {
            userContext.PhoneNumber = phoneValues.ToString();
        }

        if (headers.TryGetValue(UserContextHeaders.CompanyId, out var companyIdValues) &&
            Guid.TryParse(companyIdValues.ToString(), out var companyId))
        {
            userContext.CompanyId = companyId;
        }
        
        if (headers.TryGetValue(UserContextHeaders.EmployeeId, out var employeeIdValues) &&
            Guid.TryParse(employeeIdValues.ToString(), out var employeeId))
        {
            userContext.EmployeeId = employeeId;
        }

        if (headers.TryGetValue(UserContextHeaders.FirstName, out var firstNameValues))
        {
            userContext.FirstName = firstNameValues.ToString();
        }

        if (headers.TryGetValue(UserContextHeaders.MiddleName, out var middleNameValues))
        {
            userContext.MiddleName = middleNameValues.ToString();
        }

        if (headers.TryGetValue(UserContextHeaders.LastName, out var lastNameValues))
        {
            userContext.LastName = lastNameValues.ToString();
        }


        if (headers.TryGetValue(UserContextHeaders.Roles, out var roles))
        {
            userContext.Roles = roles;
        }
        
        if (headers.TryGetValue(UserContextHeaders.Permissions, out var permissions))
        {
            userContext.Permissions = new BitArray(Convert.FromBase64String(permissions.ToString()));
        }
        
        _userContextSetuper.Push(userContext);
        
        await _next(httpContext);
    }
    
    private static bool IsGrpcRequest(HttpRequest request)
    {
        // gRPC uses HTTP/2 and content-type "application/grpc" (often with suffixes)
        if (!HttpProtocol.IsHttp2(request.Protocol))
        {
            return false;
        }

        var contentType = request.ContentType;
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        // Covers: application/grpc, application/grpc+proto, application/grpc+json, etc.
        return contentType.StartsWith("application/grpc", StringComparison.OrdinalIgnoreCase);
    }
}
