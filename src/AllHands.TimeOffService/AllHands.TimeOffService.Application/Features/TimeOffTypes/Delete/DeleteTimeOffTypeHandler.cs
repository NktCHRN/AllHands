using AllHands.Shared.Application.Extensions;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.Delete;

public sealed class DeleteTimeOffTypeHandler(IDocumentStore documentStore, IUserContext userContext) : IRequestHandler<DeleteTimeOffTypeCommand>
{
    public async Task Handle(DeleteTimeOffTypeCommand request, CancellationToken cancellationToken)
    {
        var companyId = userContext.CompanyId;
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
        
        session.DeleteWithAuditing(type, userContext.Id);
        await session.SaveChangesAsync(cancellationToken);
    }
}
