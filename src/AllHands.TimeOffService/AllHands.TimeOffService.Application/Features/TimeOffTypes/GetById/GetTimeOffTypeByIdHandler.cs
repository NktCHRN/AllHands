using AllHands.Shared.Domain.Exceptions;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.GetById;

public sealed class GetTimeOffTypeByIdHandler(IQuerySession querySession) : IRequestHandler<GetTimeOffTypeByIdQuery, TimeOffTypeDto>
{
    public async Task<TimeOffTypeDto> Handle(GetTimeOffTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var type = await querySession.Query<TimeOffType>()
                       .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
                   ?? throw new EntityNotFoundException("Time off type was not found.");

        return new TimeOffTypeDto(type.Id, type.Order, type.Name, type.Emoji, type.DaysPerYear);
    }
}
