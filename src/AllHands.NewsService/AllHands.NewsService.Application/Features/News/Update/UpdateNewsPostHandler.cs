using AllHands.NewsService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.NewsService.Application.Features.News.Update;

public sealed class UpdateNewsPostHandler(IDocumentSession documentSession, UserContext userContext, TimeProvider timeProvider) : IRequestHandler<UpdateNewsPostCommand>
{
    public async Task Handle(UpdateNewsPostCommand request, CancellationToken cancellationToken)
    {
        var post = await documentSession.Query<NewsPost>()
                       .FirstOrDefaultAsync(p => p.Id == request.Id, token: cancellationToken)
                   ?? throw new EntityNotFoundException("News post was not found");

        if (post.AuthorId != userContext.EmployeeId)
        {
            throw new ForbiddenForUserException("You can only edit your own posts.");
        }
        
        post.Text = request.Text;
        post.UpdatedAt = timeProvider.GetUtcNow();
        documentSession.Update(post);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
