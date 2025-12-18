using AllHands.Shared.Domain.Abstractions;
using Marten;
using Marten.Patching;

namespace AllHands.Shared.Application.Extensions;

public static class MartenExtensions
{
    public static void DeleteWithAuditing<T>(this IDocumentSession session, T document, Guid deletedByUserId) where T: ISoftDeletableAuditable, IIdentifiable
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(document);
        
        session.Patch<T>(document.Id).Set(x => x.DeletedByUserId, deletedByUserId);
        session.Delete(document);
    }
}
