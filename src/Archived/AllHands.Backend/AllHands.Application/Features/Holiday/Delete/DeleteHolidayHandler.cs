using AllHands.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Holiday.Delete;

public sealed class DeleteHolidayHandler(IDocumentSession documentSession) : IRequestHandler<DeleteHolidayCommand>
{
    public async Task Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await documentSession.Query<Domain.Models.Holiday>()
                .FirstOrDefaultAsync(h => h.Id == request.Id, token: cancellationToken)
                      ?? throw new EntityNotFoundException("Holiday was not found.");
        
        documentSession.Delete(holiday);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
