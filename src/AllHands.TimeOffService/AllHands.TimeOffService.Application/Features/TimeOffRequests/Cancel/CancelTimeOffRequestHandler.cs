using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.TimeOffService.Domain.Events.TimeOff;
using AllHands.TimeOffService.Domain.Events.TimeOffBalance;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Cancel;

public sealed class CancelTimeOffRequestHandler(IDocumentSession documentSession, IUserContext userContext) : IRequestHandler<CancelTimeOffRequestCommand>
{
    public async Task Handle(CancelTimeOffRequestCommand request, CancellationToken cancellationToken)
    {
        var timeOffRequest = await documentSession.Query<TimeOffRequest>()
                                 .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                             ?? throw new EntityNotFoundException("Time off request was not found");

        if (timeOffRequest.EmployeeId != userContext.EmployeeId)
        {
            throw new ForbiddenForUserException("You can cancel only your own time off request.");
        }

        if (timeOffRequest.Status != TimeOffRequestStatus.Pending
            && timeOffRequest.Status != TimeOffRequestStatus.Approved)
        {
            throw new EntityValidationFailedException($"You cannot cancel {timeOffRequest.Status} time-off request.");
        }
        
        documentSession.Events.Append(timeOffRequest.Id, new TimeOffRequestCancelledEvent(timeOffRequest.Id, userContext.Id));
        documentSession.Events.Append(timeOffRequest.BalanceId,
            new TimeOffBalanceRequestChangeEvent(timeOffRequest.BalanceId, userContext.Id,
                timeOffRequest.Id, timeOffRequest.WorkingDaysCount));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
