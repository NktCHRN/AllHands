using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Holiday.Create;

public sealed class CreateHolidayHandler(IDocumentSession documentSession, ICurrentUserService currentUserService, TimeProvider timeProvider) : IRequestHandler<CreateHolidayCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = new Domain.Models.Holiday()
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
            Date = request.Date,
            CompanyId = currentUserService.GetCompanyId(),
            CreatedAt = timeProvider.GetUtcNow()
        };
        documentSession.Insert(holiday);
        await documentSession.SaveChangesAsync(cancellationToken);
        
        return new CreatedEntityDto(holiday.Id);
    }
}
