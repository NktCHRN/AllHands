using AllHands.Application.Dto;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Get;

public sealed class GetTimeOffRequestsHandler(IQuerySession querySession) : IRequestHandler<GetTimeOffRequestsQuery, PagedDto<TimeOffRequestDto>>
{
    public async Task<PagedDto<TimeOffRequestDto>> Handle(GetTimeOffRequestsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<TimeOffRequest> query = querySession.Query<TimeOffRequest>();

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(r => r.EmployeeId == request.EmployeeId.Value);
        }

        if (request.ManagerId.HasValue)
        {
            var employeesIds = await querySession.Query<Employee>()
                .Where(e => e.ManagerId == request.ManagerId.Value)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);
            
            query = query.Where(r => employeesIds.Contains(r.EmployeeId));
        }

        if (request.Status != TimeOffRequestStatus.Undefined)
        {
            query = query.Where(r => r.Status == request.Status);
        }

        var count = await query.CountAsync(cancellationToken);
        
        var employees = new Dictionary<Guid, Employee>();
        var approvers = new Dictionary<Guid, Employee>();
        var data = await query
            .Include(employees).On(r => r.EmployeeId)
            .Include(approvers).On(r => r.ApproverId!)
            .OrderByDescending(r => r.Id)
            .Skip((request.Page - 1) * request.PerPage)
            .Take(request.PerPage)
            .ToListAsync(token: cancellationToken);
        foreach (var timeOffRequest in data)
        {
            timeOffRequest.Employee = employees.GetValueOrDefault(timeOffRequest.EmployeeId);
            timeOffRequest.Approver = timeOffRequest.ApproverId.HasValue ? approvers.GetValueOrDefault(timeOffRequest.ApproverId.Value) : null;
        }

        return new PagedDto<TimeOffRequestDto>(data
            .Select(TimeOffRequestDto.FromModel)
            .ToList(), count);
    }
}
