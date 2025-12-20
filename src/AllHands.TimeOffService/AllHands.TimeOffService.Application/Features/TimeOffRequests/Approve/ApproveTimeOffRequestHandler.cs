using AllHands.Shared.Application.Auth;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.TimeOffService.Domain.Events.TimeOff;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Approve;

public sealed class ApproveTimeOffRequestHandler(IDocumentSession documentSession, IUserContext userContext, IUserPermissionService permissionService) : IRequestHandler<ApproveTimeOffRequestCommand>
{
    public async Task Handle(ApproveTimeOffRequestCommand request, CancellationToken cancellationToken)
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

        if (timeOffRequest.Status != TimeOffRequestStatus.Pending)
        {
            throw new EntityValidationFailedException($"You cannot approve {timeOffRequest.Status} time-off request.");
        }
        
        documentSession.Events.Append(timeOffRequest.Id, new TimeOffRequestApprovedEvent(timeOffRequest.Id, userContext.Id, userContext.EmployeeId));

        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
