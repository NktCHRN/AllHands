using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffTypes.Create;

public sealed class CreateTimeOffTypeHandler(IDocumentStore documentStore, ICurrentUserService currentUserService, TimeProvider timeProvider) : IRequestHandler<CreateTimeOffTypeCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreateTimeOffTypeCommand request, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();
        await using var session = await documentStore.LightweightSerializableSessionAsync(companyId.ToString(), cancellationToken);
        
        var lastOrder = await session.Query<TimeOffType>()
            .MaxAsync(s => s.Order, token: cancellationToken);

        var timeOffType = new TimeOffType()
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
            Emoji = request.Emoji,
            DaysPerYear = request.DaysPerYear,
            CompanyId = companyId,
            CreatedAt = timeProvider.GetUtcNow(),
            CreatedByUserId = currentUserService.GetId(),
            Order = lastOrder + 1,
        };
        
        session.Insert(timeOffType);
        await session.SaveChangesAsync(cancellationToken);

        return new CreatedEntityDto(timeOffType.Id);
    }
}
