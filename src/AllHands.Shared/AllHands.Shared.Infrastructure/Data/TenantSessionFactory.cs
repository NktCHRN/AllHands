using AllHands.Shared.Domain.UserContext;
using Marten;

namespace AllHands.Shared.Infrastructure.Data;

public sealed class TenantSessionFactory(IUserContextAccessor userContextAccessor, IDocumentStore store) : ISessionFactory
{
    public IQuerySession QuerySession()
    {
        var isCompanyIdProvided = userContextAccessor.UserContext?.CompanyId is not null
            && userContextAccessor.UserContext.CompanyId != Guid.Empty;
        
        return isCompanyIdProvided ? store.QuerySession(userContextAccessor.UserContext!.CompanyId.ToString()) : store.QuerySession();
    }

    public IDocumentSession OpenSession()
    {
        var isCompanyIdProvided = userContextAccessor.UserContext?.CompanyId is not null
                                  && userContextAccessor.UserContext.CompanyId != Guid.Empty;
        
        return isCompanyIdProvided ? store.LightweightSession(userContextAccessor.UserContext!.CompanyId.ToString()) : store.LightweightSession();
    }
}
