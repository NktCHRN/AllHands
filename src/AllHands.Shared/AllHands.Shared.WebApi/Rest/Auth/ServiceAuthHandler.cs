using System.Net.Mime;
using System.Security.Claims;
using System.Text.Encodings.Web;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.Domain.UserContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllHands.Shared.WebApi.Rest.Auth;

public sealed class ServiceAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ServiceCustom";
    
    private readonly IUserContextAccessor _userContextAccessor;
    
    public ServiceAuthHandler(IUserContextAccessor userContextAccessor, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _userContextAccessor = userContextAccessor;
    }

    public ServiceAuthHandler(IUserContextAccessor userContextAccessor, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
        _userContextAccessor = userContextAccessor;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var userId = _userContextAccessor.UserContext?.Id;
        if (userId.HasValue && userId.Value != Guid.Empty)
        {
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(
                        new ClaimsIdentity(
                            [new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())], CookieAuthenticationDefaults.AuthenticationScheme)), SchemeName)));
        }
        
        return Task.FromResult(AuthenticateResult.Fail("You are not authorized."));
    }
    
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // 401 (not authenticated / missing or invalid credentials)
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = MediaTypeNames.Application.Json;
        
        await Response.WriteAsJsonAsync(
            ApiResponse.FromError(new ErrorResponse("You are not authorized."))
        );
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        // 403 (authenticated, but not allowed by authorization policy)
        Response.StatusCode = StatusCodes.Status403Forbidden;
        Response.ContentType = MediaTypeNames.Application.Json;

        await Response.WriteAsJsonAsync(
            ApiResponse.FromError(new ErrorResponse("Access denied."))
        );
    }
}
