using AllHands.NewsService.Domain.Models;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.NewsService.Application.Features.News.Create;

public sealed class CreateNewsPostHandler(IDocumentSession documentSession, IUserContext userContext, TimeProvider timeProvider) : IRequestHandler<CreateNewsPostCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreateNewsPostCommand request, CancellationToken cancellationToken)
    {
        var post = new NewsPost()
        {
            Id = Guid.CreateVersion7(),
            Text = request.Text,
            AuthorId = userContext.EmployeeId,
            CreatedAt = timeProvider.GetUtcNow(),
            CompanyId = userContext.CompanyId,
        };
        documentSession.Insert(post);
        await documentSession.SaveChangesAsync(cancellationToken);
        
        return new CreatedEntityDto(post.Id);
    }
}
