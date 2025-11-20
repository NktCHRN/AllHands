using AllHands.Application.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AllHands.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AllHands.Cookie";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentHttpUserService>();
        
        return services;
    }
}
