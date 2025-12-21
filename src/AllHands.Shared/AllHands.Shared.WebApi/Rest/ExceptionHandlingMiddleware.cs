using System.Net;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.Domain.Exceptions;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AllHands.Shared.WebApi.Rest;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            var baseException = ex.GetBaseException();
            
            var (statusCode, message) = baseException switch
            {
                EntityAlreadyExistsException => (HttpStatusCode.Conflict, baseException.Message),
                EntityNotFoundException => (HttpStatusCode.NotFound, baseException.Message),
                EntityValidationFailedException => (HttpStatusCode.BadRequest, baseException.Message),
                UserUnauthorizedException => (HttpStatusCode.Unauthorized, baseException.Message),
                ForbiddenForUserException => (HttpStatusCode.Forbidden, baseException.Message),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occured on the server.")
            };

            if (statusCode is HttpStatusCode.InternalServerError)
            {
                _logger.LogError("An unexpected error occured: {ex}", ex);
            }

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)statusCode;
            await httpContext.Response.WriteAsJsonAsync(ApiResponse.FromError(new ErrorResponse(message)));
        }
    }
}
