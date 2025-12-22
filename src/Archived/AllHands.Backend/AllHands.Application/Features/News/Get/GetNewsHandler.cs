using AllHands.Application.Dto;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.News.Get;

public sealed class GetNewsHandler(IQuerySession querySession) : IRequestHandler<GetNewsQuery, PagedDto<NewsPostDto>>
{
    public async Task<PagedDto<NewsPostDto>> Handle(GetNewsQuery request, CancellationToken cancellationToken)
    {
        var dbQuery = querySession.Query<NewsPost>();

        var count = await dbQuery.CountAsync(cancellationToken);

        var employees = new Dictionary<Guid, Employee>();
        var posts = await dbQuery
            .Include(employees).On(p => p.AuthorId)
            .OrderByDescending(p => p.Id)
            .Skip((request.Page - 1) * request.PerPage)
            .Take(request.PerPage)
            .ToListAsync(token: cancellationToken);

        return new PagedDto<NewsPostDto>(
            posts.Select(p =>
            {
                var author = employees.GetValueOrDefault(p.AuthorId);
                return new NewsPostDto(
                    p.Id,
                    p.Text,
                    p.CreatedAt,
                    p.UpdatedAt,
                    author is null 
                        ? null 
                        : new EmployeeTitleDto(
                            p.AuthorId,
                            author.FirstName,
                            author.MiddleName,
                            author.LastName,
                            author.Email));
            }).ToList(), 
            count);
    }
}
