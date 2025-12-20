using AllHands.Shared.Application.Auth;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.TimeOffService.Domain.Events.TimeOff;
using AllHands.TimeOffService.Domain.Events.TimeOffBalance;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Reject;

public sealed class RejectTimeOffRequestHandler(IDocumentSession documentSession, IUserContext userContext, IUserPermissionService permissionService) : IRequestHandler<RejectTimeOffRequestCommand>
{
    public async Task Handle(RejectTimeOffRequestCommand request, CancellationToken cancellationToken)
    {
        var timeOffRequest = await documentSession.Query<TimeOffRequest>()
                                 .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                             ?? throw new EntityNotFoundException("Time off request was not found");
        
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.Id == timeOffRequest.EmployeeId, cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found");

        if (employee.ManagerId != userContext.EmployeeId &&
            !permissionService.IsAllowed(Permissions.TimeOffRequestAdminApprove))
        {
            throw new ForbiddenForUserException("You can only approve and reject your subordinates' requests.");
        }

        if (timeOffRequest.Status != TimeOffRequestStatus.Pending
            && timeOffRequest.Status != TimeOffRequestStatus.Approved)
        {
            throw new EntityValidationFailedException($"You cannot reject {timeOffRequest.Status} time-off request.");
        }
        
        documentSession.Events.Append(timeOffRequest.Id, new TimeOffRequestRejectedEvent(timeOffRequest.Id, userContext.Id, userContext.EmployeeId, request.Reason));
        documentSession.Events.Append(timeOffRequest.BalanceId,
            new TimeOffBalanceRequestChangeEvent(timeOffRequest.BalanceId, userContext.Id,
                timeOffRequest.Id, timeOffRequest.WorkingDaysCount));
        
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
