using AllHands.Application.Abstractions;
using AllHands.Domain.Events.TimeOff;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Approve;

public sealed class ApproveTimeOffRequestHandler(IDocumentSession documentSession, ICurrentUserService currentUserService) : IRequestHandler<ApproveTimeOffRequestCommand>
{
    public async Task Handle(ApproveTimeOffRequestCommand request, CancellationToken cancellationToken)
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

        if (timeOffRequest.Status != TimeOffRequestStatus.Pending)
        {
            throw new EntityValidationFailedException($"You cannot approve {timeOffRequest.Status} time-off request.");
        }
        
        documentSession.Events.Append(timeOffRequest.Id, new TimeOffRequestApprovedEvent(timeOffRequest.Id, currentUserService.GetId(), currentUserService.GetEmployeeId()));

        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
