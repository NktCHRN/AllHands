using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.News.Create;

public sealed class CreateNewsPostHandler(IDocumentSession documentSession, ICurrentUserService currentUserService, TimeProvider timeProvider) : IRequestHandler<CreateNewsPostCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreateNewsPostCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetId();
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.UserId == userId, token: cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found");

        var post = new NewsPost()
        {
            Id = Guid.CreateVersion7(),
            Text = request.Text,
            AuthorId = employee.Id,
            CreatedAt = timeProvider.GetUtcNow(),
            CompanyId = currentUserService.GetCompanyId(),
        };
        documentSession.Insert(post);
        await documentSession.SaveChangesAsync(cancellationToken);
        
        return new CreatedEntityDto(post.Id);
    }
}
