using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Positions.GetById;

public sealed class GetPositionByIdHandler(IQuerySession querySession) : IRequestHandler<GetPositionByIdQuery, PositionDto>
{
    public async Task<PositionDto> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
    {
        var position = await querySession.Query<Position>()
                           .Where(p => p.Id == request.Id)
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new EntityNotFoundException("Position was not found");

        return new PositionDto
        {
            Id = position.Id,
            Name = position.Name
        };
    }
}
