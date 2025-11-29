using System.Text.RegularExpressions;

namespace AllHands.WebApi;

public sealed partial class RemoveCookieOnLoginMiddleware(RequestDelegate next)
{
    [GeneratedRegex(@"^/api/v[^/]+/account/login$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex LoginPathRegex();

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var path = httpContext.Request.Path.Value ?? string.Empty;
        
        var isLogin = LoginPathRegex().IsMatch(path);
    
        if (isLogin)
        {
            // Remove all cookies from the request
            httpContext.Request.Headers.Remove("Cookie");
        }

        await next(httpContext);
    }
}
