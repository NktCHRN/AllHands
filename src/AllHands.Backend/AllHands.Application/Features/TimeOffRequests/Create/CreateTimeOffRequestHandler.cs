using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Abstractions;
using AllHands.Domain.Events.TimeOff;
using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Create;

public sealed class CreateTimeOffRequestHandler(IDocumentSession documentSession, ICurrentUserService currentUserService, IWorkDaysCalculator workDaysCalculator) : IRequestHandler<CreateTimeOffRequestCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreateTimeOffRequestCommand request, CancellationToken cancellationToken)
    {
        var employeeId = currentUserService.GetEmployeeId();
        var timeOffTypeExists = await documentSession.Query<TimeOffType>()
            .AnyAsync(t => t.Id == request.TypeId, cancellationToken);

        if (!timeOffTypeExists)
        {
            throw new EntityNotFoundException("Time off type was not found.");
        }

        var balance = await documentSession.Query<TimeOffBalance>()
            .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.TypeId == request.TypeId,
                cancellationToken);
        var balanceId = balance?.Id;
        
        if (balance == null)
        {
            balanceId = TimeOffBalance.CreateId(employeeId, request.TypeId);
            documentSession.Events.StartStream(balanceId.Value, new TimeOffBalanceCreatedEvent(balanceId.Value, employeeId, request.TypeId, 0));
        }

        var company = await documentSession.Query<Domain.Models.Company>()
            .FirstOrDefaultAsync(token: cancellationToken)
            ?? throw new EntityNotFoundException("Company was not found.");
        var holidays = await documentSession.Query<Domain.Models.Holiday>()
            .Where(h => h.Date >= request.StartDate && h.Date <= request.EndDate)
            .ToListAsync(token: cancellationToken);
        
        var workDaysCount = workDaysCalculator.Calculate(request.StartDate, request.EndDate, company, holidays);

        var timeOffRequestId = Guid.CreateVersion7();
        var @event = new TimeOffRequestedEvent(
            timeOffRequestId, 
            currentUserService.GetId(), 
            employeeId,
            currentUserService.GetCompanyId(),
            request.TypeId,
            request.StartDate,
            request.EndDate,
            workDaysCount,
            balanceId!.Value);
        documentSession.Events.StartStream(timeOffRequestId, @event);
        
        documentSession.Events.Append(balanceId.Value, new TimeOffBalanceRequestChangeEvent(balanceId.Value, currentUserService.GetId(), timeOffRequestId, -workDaysCount));
        
        await documentSession.SaveChangesAsync(cancellationToken);

        return new CreatedEntityDto(timeOffRequestId);
    }
}
