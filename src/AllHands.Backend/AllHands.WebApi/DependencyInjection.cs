using System.Text.Json.Serialization;
using AllHands.Application.Abstractions;
using AllHands.WebApi.Contracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AllHands.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
            .BindConfiguration("CookieAuthenticationOptions")
            .Configure<ITicketStore>((options, ticketStore) =>
            {
                options.SessionStore = ticketStore;
            });
        services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(opt =>
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

        services.AddAuthorization();
        services.AddOptions<AuthorizationOptions>()
            .Configure<IServiceProvider>((options, sp) =>
            {
                var permissions = sp.GetRequiredService<IPermissionsContainer>().Permissions;
                foreach (var (permission, _) in permissions)
                {
                    options.AddPolicy($"HasPermission_{permission}", policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });

        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentHttpUserService>();
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
                .AllowAnyHeader()
                .AllowCredentials()));
        }
        else
        {
            services.AddCors(opt => opt.AddPolicy("DEV_CORS", p => p
                .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()));
        }
        
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            
            options.AssumeDefaultVersionWhenUnspecified = true;
            
            options.ReportApiVersions = true;
            
            options.ApiVersionReader = ApiVersionReader.Combine(
                // URL segment versioning: /api/v1/...
                new UrlSegmentApiVersionReader(),
                // or via header: x-api-version: 1.0
                new HeaderApiVersionReader("x-api-version"),
                // or via query string: ?api-version=1.0
                new QueryStringApiVersionReader("api-version")
            );
        }).AddApiExplorer(options =>
        {
            // Format the group names for Swagger documents (e.g., 'v1', 'v2')
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true; // Replace version placeholder in URLs
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        return services;
    }
}
