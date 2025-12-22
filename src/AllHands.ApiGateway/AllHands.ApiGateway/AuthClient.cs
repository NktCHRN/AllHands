using System.Net;

namespace AllHands.ApiGateway;

public sealed class AuthClient(HttpClient httpClient)
{
    public async Task<HttpResponseMessage> AuthenticateAsync(string cookie, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/v1/account/authenticate");
        
        request.Headers.Add("Cookie", cookie);

        var result = await httpClient.SendAsync(request, cancellationToken: cancellationToken);

        return result;
    }
}
