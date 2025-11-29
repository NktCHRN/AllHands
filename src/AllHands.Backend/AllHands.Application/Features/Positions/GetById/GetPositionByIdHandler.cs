using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Positions.GetById;

public sealed class GetPositionByIdHandler(IQuerySession querySession) : IRequestHandler<GetPositionByIdQuery, PositionDto>
{
    public async Task<PositionDto> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
    {
        var position = await querySession.Query<Position>()
                           .Where(p => p.Id == request.Id && !p.DeletedAt.HasValue)
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new EntityNotFoundException("Position was not found");

        return new PositionDto
        {
            Id = position.Id,
            Name = position.Name
        };
    }
}
