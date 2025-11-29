using AllHands.Application.Abstractions;
using Marten;

namespace AllHands.Infrastructure.Data;

public sealed class TenantSessionFactory(ICurrentUserService currentUserService, IDocumentStore store) : ISessionFactory
{
    public IQuerySession QuerySession()
    {
        var isCompanyIdProvided = currentUserService.TryGetCompanyId(out var companyId);
        
        return isCompanyIdProvided ? store.QuerySession(companyId.ToString()) : store.QuerySession();
    }

    public IDocumentSession OpenSession()
    {
        var isCompanyIdProvided = currentUserService.TryGetCompanyId(out var companyId);
        
        return isCompanyIdProvided ? store.LightweightSession(companyId.ToString()) : store.LightweightSession();
    }
}
