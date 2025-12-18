using System.Net;

namespace AllHands.Shared.Infrastructure.Utilities;

public static class HttpResponseUtility
{
    public static bool IsSuccess(HttpStatusCode statusCode)
        => (int)statusCode >= 200 && (int)statusCode <= 299;
}
