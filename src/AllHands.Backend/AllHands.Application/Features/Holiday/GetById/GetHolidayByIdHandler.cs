using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Holiday.GetById;

public sealed class GetHolidayByIdHandler(IQuerySession querySession) : IRequestHandler<GetHolidayByIdQuery, HolidayDto>
{
    public async Task<HolidayDto> Handle(GetHolidayByIdQuery request, CancellationToken cancellationToken)
    {
        var holiday = await querySession.Query<Domain.Models.Holiday>()
                          .FirstOrDefaultAsync(h => h.Id == request.Id, token: cancellationToken)
                      ?? throw new EntityNotFoundException("Holiday was not found.");
        
        return new HolidayDto(holiday.Id, holiday.Name, holiday.Date);
    }
}
