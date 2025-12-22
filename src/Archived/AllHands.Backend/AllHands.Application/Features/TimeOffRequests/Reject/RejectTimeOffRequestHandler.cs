using AllHands.Application.Abstractions;
using AllHands.Domain.Events.TimeOff;
using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Reject;

public sealed class RejectTimeOffRequestHandler(IDocumentSession documentSession, ICurrentUserService currentUserService) : IRequestHandler<RejectTimeOffRequestCommand>
{
    public async Task Handle(RejectTimeOffRequestCommand request, CancellationToken cancellationToken)
    {
        var timeOffRequest = await documentSession.Query<TimeOffRequest>()
                                 .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                             ?? throw new EntityNotFoundException("Time off request was not found");
        
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.Id == timeOffRequest.EmployeeId, cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found");

        if (employee.ManagerId != currentUserService.GetEmployeeId() &&
            !currentUserService.IsAllowed(Application.Permissions.TimeOffRequestAdminApprove))
        {
            throw new ForbiddenForUserException("You can only approve and reject your subordinates' requests.");
        }

        if (timeOffRequest.Status != TimeOffRequestStatus.Pending
            && timeOffRequest.Status != TimeOffRequestStatus.Approved)
        {
            throw new EntityValidationFailedException($"You cannot reject {timeOffRequest.Status} time-off request.");
        }
        
        documentSession.Events.Append(timeOffRequest.Id, new TimeOffRequestRejectedEvent(timeOffRequest.Id, currentUserService.GetId(), currentUserService.GetEmployeeId(), request.Reason));
        documentSession.Events.Append(timeOffRequest.BalanceId,
            new TimeOffBalanceRequestChangeEvent(timeOffRequest.BalanceId, currentUserService.GetId(),
                timeOffRequest.Id, timeOffRequest.WorkingDaysCount));
        
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
