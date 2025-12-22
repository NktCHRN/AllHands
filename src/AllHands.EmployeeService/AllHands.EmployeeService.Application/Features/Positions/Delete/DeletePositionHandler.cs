using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Application.Extensions;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Positions.Delete;

public sealed class DeletePositionHandler(IDocumentSession documentSession, IUserContext userContext) : IRequestHandler<DeletePositionCommand>
{
    public async Task Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var existingPosition = await documentSession.Query<Position>()
                                   .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                               ?? throw new EntityNotFoundException("Position not found");
        
        documentSession.DeleteWithAuditing(existingPosition, userContext.Id);
        
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
