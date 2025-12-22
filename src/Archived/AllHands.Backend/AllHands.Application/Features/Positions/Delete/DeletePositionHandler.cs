using AllHands.Application.Abstractions;
using AllHands.Application.Extensions;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Positions.Delete;

public sealed class DeletePositionHandler(IDocumentSession documentSession, ICurrentUserService currentUserService) : IRequestHandler<DeletePositionCommand>
{
    public async Task Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var existingPosition = await documentSession.Query<Position>()
                                   .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                               ?? throw new EntityNotFoundException("Position not found");
        
        documentSession.DeleteWithAuditing(existingPosition, currentUserService.GetId());
        
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
