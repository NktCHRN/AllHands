using AllHands.Application.Abstractions;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.News.Update;

public sealed class UpdateNewsPostHandler(IDocumentSession documentSession, ICurrentUserService currentUserService, TimeProvider timeProvider) : IRequestHandler<UpdateNewsPostCommand>
{
    public async Task Handle(UpdateNewsPostCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetId();
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.UserId == userId, token: cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found");

        var post = await documentSession.Query<NewsPost>()
                       .FirstOrDefaultAsync(p => p.Id == request.Id, token: cancellationToken)
                   ?? throw new EntityNotFoundException("News post was not found");

        if (post.AuthorId != employee.Id)
        {
            throw new ForbiddenForUserException("You can only edit your own posts.");
        }
        
        post.Text = request.Text;
        post.UpdatedAt = timeProvider.GetUtcNow();
        documentSession.Update(post);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
