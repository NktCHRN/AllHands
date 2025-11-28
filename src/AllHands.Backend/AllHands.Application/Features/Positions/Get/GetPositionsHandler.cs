using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Models;
using AllHands.Domain.Utilities;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Positions.Get;

public sealed class GetPositionsHandler(ICurrentUserService currentUserService, IQuerySession querySession) : IRequestHandler<GetPositionsQuery, PagedDto<PositionDto>>
{
    public async Task<PagedDto<PositionDto>> Handle(GetPositionsQuery request, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();

        IQueryable<Position> positionsQuery = querySession.Query<Position>()
            .Where(p => !p.DeletedAt.HasValue);

        if (!string.IsNullOrEmpty(request.Search))
        {
            var normalizedSearch = StringUtilities.GetNormalizedName(request.Search);
            positionsQuery = positionsQuery.Where(p => p.NormalizedName.Contains(normalizedSearch));
        }
        
        var count = await positionsQuery.CountAsync(cancellationToken);
        
        var positions = await positionsQuery.OrderBy(p => p.NormalizedName)
            .Skip((request.Page - 1) * request.PerPage)
            .Take(request.PerPage)
            .ToListAsync(token: cancellationToken);

        return new PagedDto<PositionDto>(positions
            .Select(p => new PositionDto
            {
                Id = p.Id, 
                Name = p.Name
            })
            .ToList(), count);
    }
}
