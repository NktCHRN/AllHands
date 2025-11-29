using AllHands.Domain.Abstractions;
using AllHands.Domain.Models;
using Marten;
using Marten.Patching;

namespace AllHands.Application.Extensions;

public static class MartenExtensions
{
    public static void DeleteWithAuditing<T>(this IDocumentSession session, T document, Guid deletedByUserId) where T: ISoftDeletable, IIdentifiable
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(document);
        
        session.Patch<Position>(document.Id).Set(x => x.DeletedByUserId, deletedByUserId);
        session.Delete(document);
    }
}
