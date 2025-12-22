using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using AllHands.Domain.Utilities;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Positions.Create;

public sealed class CreatePositionHandler(IDocumentSession documentSession, ICurrentUserService currentUserService, TimeProvider timeProvider) : IRequestHandler<CreatePositionCommand, CreatedEntityDto>
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
            CompanyId = currentUserService.GetCompanyId(),
            CreatedAt = timeProvider.GetUtcNow(),
            CreatedByUserId = currentUserService.GetId()
        };
        documentSession.Insert(position);
        await documentSession.SaveChangesAsync(cancellationToken);
        
        return new CreatedEntityDto(position.Id);
    }
}
