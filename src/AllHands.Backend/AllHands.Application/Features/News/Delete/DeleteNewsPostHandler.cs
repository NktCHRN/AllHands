using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.News.Delete;

public sealed class DeleteNewsPostHandler(IDocumentSession documentSession) : IRequestHandler<DeleteNewsPostCommand>
{
    public async Task Handle(DeleteNewsPostCommand request, CancellationToken cancellationToken)
    {
        var post = await documentSession.LoadAsync<NewsPost>(request.Id, cancellationToken)
                   ?? throw new EntityNotFoundException("NewsPost was not found");
        
        documentSession.Delete(post);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
