using AllHands.Application.Abstractions;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using AllHands.Domain.Utilities;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Positions.Update;

public sealed class UpdatePositionHandler(ICurrentUserService currentUserService, IDocumentSession documentSession, TimeProvider timeProvider) : IRequestHandler<UpdatePositionCommand>
{
    public async Task Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = StringUtilities.GetNormalizedName(request.Name);
        
        var existingPosition = await documentSession.Query<Position>()
                                   .FirstOrDefaultAsync(p => p.Id == request.Id && !p.DeletedAt.HasValue, cancellationToken)
            ?? throw new EntityNotFoundException("Position not found");

        if (existingPosition.NormalizedName != normalizedName)
        {
            var positionExists = await documentSession.Query<Position>()
                .AnyAsync(x => x.NormalizedName == normalizedName && !x.DeletedAt.HasValue, token: cancellationToken);

            if (positionExists)
            {
                throw new EntityAlreadyExistsException("Another position with this name already exists");   
            }
        }
        
        existingPosition.Name = request.Name;
        existingPosition.NormalizedName = normalizedName;
        existingPosition.UpdatedAt = timeProvider.GetUtcNow();
        existingPosition.UpdatedByUserId = currentUserService.GetId();
        
        documentSession.Update(existingPosition);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
