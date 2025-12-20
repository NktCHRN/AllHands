using System.Text.Json.Serialization;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.WebApi.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.RateLimiting;

namespace AllHands.AuthService.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        
        services.AddOpenApi();

        if (environment.IsDevelopment())
        {
            services.AddCors(opt => opt.AddPolicy("CORS", p => p
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
        }
        else
        {
            services.AddCors(opt => opt.AddPolicy("CORS", p => p
                .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()));
        }
        
        services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
            .BindConfiguration("CookieAuthenticationOptions")
            .Configure<ITicketStore>((options, ticketStore) =>
            {
                options.SessionStore = ticketStore;
            });
        services.AddPermissionBasedAuth().AddCookie(opt =>
        {
            opt.Events.OnRedirectToLogin = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(ApiResponse.FromError(new ErrorResponse("You are not authorized.")));
            };
            opt.Events.OnRedirectToAccessDenied = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(ApiResponse.FromError(new ErrorResponse("Access denied.")));
            };
        });

        services.AddAllHandsVersioning();
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("ResetPasswordLimiter", o =>
            {
                o.Window = TimeSpan.FromSeconds(10);
                o.PermitLimit = 5;
                o.QueueLimit = 0;
            });
        });
        
        services.AddHealthChecks();

        return services;
    }
}