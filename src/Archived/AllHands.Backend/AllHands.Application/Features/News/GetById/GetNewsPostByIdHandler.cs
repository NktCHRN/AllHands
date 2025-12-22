using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.News.GetById;

public sealed class GetNewsPostByIdHandler(IQuerySession querySession) : IRequestHandler<GetNewsPostByIdQuery, NewsPostDto>
{
    public async Task<NewsPostDto> Handle(GetNewsPostByIdQuery request, CancellationToken cancellationToken)
    {
        Employee? author = null;
        var post = await querySession.Query<NewsPost>()
            .Include<Employee>(e => author = e).On(n => n.AuthorId)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Post was not found");
        
        return new NewsPostDto(
            post.Id,
            post.Text,
            post.CreatedAt,
            post.UpdatedAt,
            author is null 
                ? null 
                : new EmployeeTitleDto(
                    post.AuthorId,
                    author.FirstName,
                    author.MiddleName,
                    author.LastName,
                    author.Email));
    }
}
