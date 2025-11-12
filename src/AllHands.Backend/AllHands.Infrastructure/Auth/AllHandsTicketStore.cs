using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace AllHands.Infrastructure.Auth;

public sealed class AllHandsTicketStore(AuthDbContext dbContext, IDistributedCache cache) : ITicketStore
{
    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        throw new NotImplementedException();
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        throw new NotImplementedException();
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key)
    {
        throw new NotImplementedException();
    }
}
