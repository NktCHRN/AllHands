using AllHands.Shared.Application.Dto;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.Holiday.Create;

public sealed class CreateHolidayHandler(IDocumentSession documentSession, IUserContext userContext, TimeProvider timeProvider) : IRequestHandler<CreateHolidayCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = new Domain.Models.Holiday()
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
            Date = request.Date,
            CompanyId = userContext.CompanyId,
            CreatedAt = timeProvider.GetUtcNow()
        };
        documentSession.Insert(holiday);
        await documentSession.SaveChangesAsync(cancellationToken);
        
        return new CreatedEntityDto(holiday.Id);
    }
}
