using System.Globalization;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.TimeOffService.Domain.Abstractions;
using AllHands.TimeOffService.Domain.Events.TimeOff;
using AllHands.TimeOffService.Domain.Events.TimeOffBalance;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Create;

public sealed class CreateTimeOffRequestHandler(IDocumentSession documentSession, IUserContext userContext, IWorkDaysCalculator workDaysCalculator) : IRequestHandler<CreateTimeOffRequestCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreateTimeOffRequestCommand request, CancellationToken cancellationToken)
    {
        var employeeId = userContext.EmployeeId;
        var timeOffTypeExists = await documentSession.Query<TimeOffType>()
            .AnyAsync(t => t.Id == request.TypeId, cancellationToken);

        if (!timeOffTypeExists)
        {
            throw new EntityNotFoundException("Time off type was not found.");
        }
        
        var overlappingRequest = await documentSession.Query<TimeOffRequest>()
            .FirstOrDefaultAsync(r => r.EmployeeId == employeeId 
                                      && (r.Status == TimeOffRequestStatus.Pending || r.Status == TimeOffRequestStatus.Approved)
                && r.StartDate <= request.EndDate && r.EndDate >= request.StartDate, token: cancellationToken);
        if (overlappingRequest != null)
        {
            throw new EntityAlreadyExistsException($"You have an overlapping request from {overlappingRequest.StartDate.ToString("o", CultureInfo.InvariantCulture)} to {overlappingRequest.EndDate.ToString("o", CultureInfo.InvariantCulture)}.");
        }
        
        var balance = await documentSession.Query<TimeOffBalance>()
            .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.TypeId == request.TypeId,
                cancellationToken);
        var balanceId = balance?.Id;
        
        if (balance == null)
        {
            balanceId = TimeOffBalance.CreateId(employeeId, request.TypeId);
            documentSession.Events.StartStream<TimeOffBalance>(balanceId.Value, new TimeOffBalanceCreatedEvent(balanceId.Value, employeeId, request.TypeId, 0));
        }

        var company = await documentSession.Query<Domain.Models.Company>()
            .FirstOrDefaultAsync(token: cancellationToken)
            ?? throw new EntityNotFoundException("Company was not found.");
        var holidays = await documentSession.Query<Domain.Models.Holiday>()
            .Where(h => h.Date >= request.StartDate && h.Date <= request.EndDate)
            .ToListAsync(token: cancellationToken);
        
        var workDaysCount = workDaysCalculator.Calculate(request.StartDate, request.EndDate, company, holidays);
        if (workDaysCount == 0)
        {
            throw new EntityValidationFailedException("You cannot take time off for 0 days.");
        }

        var timeOffRequestId = Guid.CreateVersion7();
        var @event = new TimeOffRequestedEvent(
            timeOffRequestId, 
            userContext.Id, 
            employeeId,
            userContext.CompanyId,
            request.TypeId,
            request.StartDate,
            request.EndDate,
            workDaysCount,
            balanceId!.Value);
        documentSession.Events.StartStream<TimeOffRequest>(timeOffRequestId, @event);
        
        documentSession.Events.Append(balanceId.Value, new TimeOffBalanceRequestChangeEvent(balanceId.Value, userContext.Id, timeOffRequestId, -workDaysCount));
        
        await documentSession.SaveChangesAsync(cancellationToken);

        return new CreatedEntityDto(timeOffRequestId);
    }
}
