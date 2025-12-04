using AllHands.Application.Dto;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.GetInTimeOff;

public sealed class GetEmployeesInTimeOffHandler(IQuerySession querySession) : IRequestHandler<GetEmployeesInTimeOffQuery, GetEmployeesInTimeOffResult>
{
    public async Task<GetEmployeesInTimeOffResult> Handle(GetEmployeesInTimeOffQuery request, CancellationToken cancellationToken)
    {
        var employees = new Dictionary<Guid, Employee>();
        var timeOffRequests = await querySession.Query<TimeOffRequest>()
            .Include(employees).On(r => r.EmployeeId)
            .Where(r => r.StartDate <= request.End && r.EndDate >= request.Start
                && (r.Status == TimeOffRequestStatus.Pending || r.Status == TimeOffRequestStatus.Approved))
            .ToListAsync(cancellationToken);

        var resultItems = new List<GetEmployeesInTimeOffResultItem>();
        var current = request.Start;
        while (current <= request.End)
        {
            var timeOffRequestsForCurrentDay = timeOffRequests
                .Where(r => r.StartDate <= current && r.EndDate >= current)
                .ToList();
            var employeesForCurrentDay = new List<EmployeeInTimeOffDto>();
            foreach (var timeOffRequest in timeOffRequestsForCurrentDay)
            {
                var employee = employees.GetValueOrDefault(timeOffRequest.EmployeeId);
                if (employee != null)
                {
                    employeesForCurrentDay.Add(new EmployeeInTimeOffDto(EmployeeTitleDto.FromModel(employee), timeOffRequest.StartDate, timeOffRequest.EndDate));
                }
            }
            var item = new GetEmployeesInTimeOffResultItem(
                current,
                employeesForCurrentDay);
            resultItems.Add(item);
            current = current.AddDays(1);
        }

        return new GetEmployeesInTimeOffResult(resultItems);
    }
}
