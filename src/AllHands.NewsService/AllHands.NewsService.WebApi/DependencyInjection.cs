using System.Text.Json.Serialization;
using AllHands.Shared.WebApi.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AllHands.NewsService.WebApi;

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
        
        services.AddPermissionBasedAuth();

        services.AddAllHandsVersioning();
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddHealthChecks();

        return services;
    }
}
