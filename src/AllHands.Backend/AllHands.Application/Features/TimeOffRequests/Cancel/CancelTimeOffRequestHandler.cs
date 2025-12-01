using AllHands.Application.Abstractions;
using AllHands.Domain.Events.TimeOff;
using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Cancel;

public sealed class CancelTimeOffRequestHandler(IDocumentSession documentSession, ICurrentUserService currentUserService) : IRequestHandler<CancelTimeOffRequestCommand>
{
    public async Task Handle(CancelTimeOffRequestCommand request, CancellationToken cancellationToken)
    {
        var timeOffRequest = await documentSession.Query<TimeOffRequest>()
                                 .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                             ?? throw new EntityNotFoundException("Time off request was not found");

        if (timeOffRequest.EmployeeId != currentUserService.GetEmployeeId())
        {
            throw new ForbiddenForUserException("You can cancel only your own time off request.");
        }

        if (timeOffRequest.Status != TimeOffRequestStatus.Pending
            && timeOffRequest.Status != TimeOffRequestStatus.Approved)
        {
            throw new EntityValidationFailedException($"You cannot cancel {timeOffRequest.Status} time-off request.");
        }
        
        documentSession.Events.Append(timeOffRequest.Id, new TimeOffRequestCancelledEvent(timeOffRequest.Id, currentUserService.GetId()));
        documentSession.Events.Append(timeOffRequest.BalanceId,
            new TimeOffBalanceRequestChangeEvent(timeOffRequest.BalanceId, currentUserService.GetId(),
                timeOffRequest.Id, timeOffRequest.WorkingDaysCount));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
