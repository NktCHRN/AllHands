using System.Collections.Frozen;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.Infrastructure.UserContext;

namespace AllHands.ApiGateway;

public sealed class UserContextHeadersSetterMiddleware
{
    private readonly RequestDelegate _next;

    private readonly FrozenSet<string> _noAuthPaths =
    [
        "/api/v*/account/login",
        "/api/v*/account/logout",
        "/api/v*/account/register/invitation",
        "/api/v*/account/reset-password",
    ];

    private readonly FrozenSet<string> _sendCookiePaths = 
    [
        "/api/v*/account/login",
        "/api/v*/account/relogin",
        "/api/v*/account/logout"
    ];

    private static readonly string[] _userContextHeaders =
    [
        UserContextHeaders.Id,
        UserContextHeaders.Email,
        UserContextHeaders.CompanyId,
        UserContextHeaders.FirstName,
        UserContextHeaders.MiddleName,
        UserContextHeaders.LastName,
        UserContextHeaders.PhoneNumber,
        UserContextHeaders.EmployeeId,
        UserContextHeaders.Roles,
        UserContextHeaders.Permissions
    ];
    
    public UserContextHeadersSetterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, AuthClient authClient)
    {
        for (var i = 0; i < _userContextHeaders.Length; i++)
        {
            httpContext.Request.Headers.Remove(_userContextHeaders[i]);
        }
        
        var normalizedPath = Normalize(httpContext.Request.Path);
        
        var cookie = httpContext.Request.Headers.Cookie.ToString();

        if (!_sendCookiePaths.Contains(normalizedPath))
        {
            httpContext.Request.Headers.Cookie = [];
        }

        if (_noAuthPaths.Contains(normalizedPath))
        {
            await _next(httpContext);
            return;
        }
        
        if (string.IsNullOrEmpty(cookie))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(ApiResponse.FromError(new ErrorResponse("You are not authorized.")));
            return;
        }
        
        var authResult = await authClient.AuthenticateAsync(cookie);

        if ((int) authResult.StatusCode < 200 || (int) authResult.StatusCode > 299)
        {
            httpContext.Response.StatusCode = (int)authResult.StatusCode;
            httpContext.Response.ContentType = authResult.Content.Headers.ContentType?.ToString();
            var body = await authResult.Content.ReadAsStringAsync();
            await httpContext.Response.WriteAsync(body);
            return;
        }
        
        for (var i = 0; i < _userContextHeaders.Length; i++)
        {
            CopyHeader(authResult, httpContext.Request.Headers, _userContextHeaders[i]);
        }
        
        await _next(httpContext);
    }
    
    private static string Normalize(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // Must be at least: /api/vX/...
        if (segments.Length < 3 || !segments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
            return path;

        if (!segments[1].StartsWith("v", StringComparison.OrdinalIgnoreCase))
            return path;

        segments[1] = "v*";

        return "/" + string.Join('/', segments);
    }
    
    private static void CopyHeader(
        HttpResponseMessage authResult,
        IHeaderDictionary targetHeaders,
        string headerName)
    {
        if (authResult.Headers.TryGetValues(headerName, out var values) ||
            authResult.Content.Headers.TryGetValues(headerName, out values))
        {
            targetHeaders[headerName] = values.ToArray();
        }
    }
}
