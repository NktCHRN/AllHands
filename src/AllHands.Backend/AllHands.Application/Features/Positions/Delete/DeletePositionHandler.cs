using AllHands.Application.Abstractions;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Positions.Delete;

public sealed class DeletePositionHandler(IDocumentSession documentSession, ICurrentUserService currentUserService, TimeProvider timeProvider) : IRequestHandler<DeletePositionCommand>
{
    public async Task Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var existingPosition = await documentSession.Query<Position>()
                                   .FirstOrDefaultAsync(p => p.Id == request.Id && !p.DeletedAt.HasValue, cancellationToken)
                               ?? throw new EntityNotFoundException("Position not found");
        
        existingPosition.DeletedAt = timeProvider.GetUtcNow();
        existingPosition.DeletedByUserId = currentUserService.GetId();
        
        documentSession.Update(existingPosition);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
