using AllHands.Application.Abstractions;
using AllHands.Application.Extensions;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffTypes.Delete;

public sealed class DeleteTimeOffTypeHandler(IDocumentStore documentStore, ICurrentUserService currentUserService) : IRequestHandler<DeleteTimeOffTypeCommand>
{
    public async Task Handle(DeleteTimeOffTypeCommand request, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();
        await using var session = await documentStore.LightweightSerializableSessionAsync(companyId.ToString(), cancellationToken);
        
        var types = await session.Query<TimeOffType>()
            .ToListAsync(token: cancellationToken);
        var type = types.FirstOrDefault(t => t.Id == request.Id)
            ?? throw new EntityNotFoundException("Time off type was not found.");

        foreach (var timeOffType in types)
        {
            if (timeOffType.Order <= type.Order)
            {
                continue;
            }
            
            timeOffType.Order--;
            session.Update(timeOffType);
        }
        
        session.DeleteWithAuditing(type, currentUserService.GetId());
        await session.SaveChangesAsync(cancellationToken);
    }
}
