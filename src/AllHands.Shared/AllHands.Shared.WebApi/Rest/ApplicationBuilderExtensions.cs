using Microsoft.AspNetCore.Builder;

namespace AllHands.Shared.WebApi.Rest;

public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseExceptionHandlingMiddleware()
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        
            return app;
        }

        public IApplicationBuilder UseUserContextHeadersMiddleware()
        {
            app.UseMiddleware<UserContextHeadersMiddleware>();
        
            return app;
        }
    }
}
