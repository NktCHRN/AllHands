using AllHands.Shared.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.Holiday.Update;

public sealed class UpdateHolidayHandler(IDocumentSession documentSession, TimeProvider timeProvider) : IRequestHandler<UpdateHolidayCommand>
{
    public async Task Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await documentSession.Query<Domain.Models.Holiday>()
                          .FirstOrDefaultAsync(h => h.Id == request.Id, token: cancellationToken)
                      ?? throw new EntityNotFoundException("Holiday was not found.");
        
        holiday.Date = request.Date;
        holiday.Name = request.Name;
        holiday.UpdatedAt = timeProvider.GetUtcNow();
        
        documentSession.Update(holiday);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
