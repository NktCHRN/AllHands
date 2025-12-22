using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Domain.Utilities;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Positions.Update;

public sealed class UpdatePositionHandler(IUserContext userContext, IDocumentSession documentSession, TimeProvider timeProvider) : IRequestHandler<UpdatePositionCommand>
{
    public async Task Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = StringUtilities.GetNormalizedName(request.Name);
        
        var existingPosition = await documentSession.Query<Position>()
                                   .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Position not found");

        if (existingPosition.NormalizedName != normalizedName)
        {
            var positionExists = await documentSession.Query<Position>()
                .AnyAsync(x => x.NormalizedName == normalizedName, token: cancellationToken);

            if (positionExists)
            {
                throw new EntityAlreadyExistsException("Another position with this name already exists");   
            }
        }
        
        existingPosition.Name = request.Name;
        existingPosition.NormalizedName = normalizedName;
        existingPosition.UpdatedAt = timeProvider.GetUtcNow();
        existingPosition.UpdatedByUserId = userContext.Id;
        
        documentSession.Update(existingPosition);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
