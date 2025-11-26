using System.Text.Json.Serialization;
using AllHands.Application.Abstractions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AllHands.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
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
