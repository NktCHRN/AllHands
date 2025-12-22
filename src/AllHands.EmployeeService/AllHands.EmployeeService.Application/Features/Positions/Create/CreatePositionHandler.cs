using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Domain.Utilities;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Positions.Create;

public sealed class CreatePositionHandler(IDocumentSession documentSession, IUserContext userContext, TimeProvider timeProvider) : IRequestHandler<CreatePositionCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = StringUtilities.GetNormalizedName(request.Name);
        
        var positionExists = await documentSession.Query<Position>()
            .AnyAsync(x => x.NormalizedName == normalizedName, token: cancellationToken);

        if (positionExists)
        {
            throw new EntityAlreadyExistsException("Position already exists");
        }

        var position = new Position()
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
            NormalizedName = StringUtilities.GetNormalizedName(request.Name),
            CompanyId = userContext.CompanyId,
            CreatedAt = timeProvider.GetUtcNow(),
            CreatedByUserId = userContext.Id
        };
        documentSession.Insert(position);
        await documentSession.SaveChangesAsync(cancellationToken);
        
        return new CreatedEntityDto(position.Id);
    }
}
