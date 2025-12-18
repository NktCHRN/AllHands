using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.Shared.WebApi.Rest;

public static class DependencyInjection
{
    public static IServiceCollection AddAllHandsVersioning(this IServiceCollection services)
    {
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
        
        return services;
    }
}
