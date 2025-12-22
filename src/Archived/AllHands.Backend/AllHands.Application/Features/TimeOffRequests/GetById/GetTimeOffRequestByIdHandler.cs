using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.GetById;

public sealed class GetTimeOffRequestByIdHandler(IQuerySession querySession) : IRequestHandler<GetTimeOffRequestByIdQuery, TimeOffRequestDto>
{
    public async Task<TimeOffRequestDto> Handle(GetTimeOffRequestByIdQuery request, CancellationToken cancellationToken)
    {
        Employee? employee = null;
        Employee? approver = null;
        TimeOffType? timeOffType = null;

        var timeOffRequest = await querySession.Query<TimeOffRequest>()
            .Include<Employee>(r => employee = r).On(r => r.EmployeeId)
            .Include<Employee>(r => approver = r).On(r => r.ApproverId!)
            .Include<TimeOffType>(t => timeOffType = t).On(r => r.TypeId!)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Time off request was not found");
        timeOffRequest.Employee = employee;
        timeOffRequest.Approver = approver;
        timeOffRequest.Type = timeOffType;
        
        return TimeOffRequestDto.FromModel(timeOffRequest);
    }
}
