using AllHands.NewsService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.NewsService.Application.Features.News.Delete;

public sealed class DeleteNewsPostHandler(IDocumentSession documentSession) : IRequestHandler<DeleteNewsPostCommand>
{
    public async Task Handle(DeleteNewsPostCommand request, CancellationToken cancellationToken)
    {
        var post = await documentSession.Query<NewsPost>()
                .FirstOrDefaultAsync(p => p.Id == request.Id, token: cancellationToken)
                   ?? throw new EntityNotFoundException("NewsPost was not found");
        
        documentSession.Delete(post);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
