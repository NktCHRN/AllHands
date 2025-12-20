using AllHands.Shared.Application.Dto;
using AllHands.TimeOffService.Application.Dto;
using AllHands.TimeOffService.Domain.Events.TimeOffBalance;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using Marten.Events;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;

public sealed class GetTimeOffBalancesHistoryHandler(IQuerySession querySession) : IRequestHandler<GetTimeOffBalancesHistoryQuery, PagedDto<TimeOffBalancesHistoryItemDto>>
{
    public async Task<PagedDto<TimeOffBalancesHistoryItemDto>> Handle(GetTimeOffBalancesHistoryQuery request, CancellationToken cancellationToken)
    {
        var balanceTypes = new Dictionary<Guid, TimeOffType>();
        var balances = await querySession.Query<TimeOffBalance>()
            .Include(balanceTypes).On(b => b.TypeId)
            .Where(b => b.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);
        var balancesDict = balances.ToDictionary(b => b.Id, b => b);
        foreach (var b in balances)
        {
            b.Type = balanceTypes.GetValueOrDefault(b.TypeId);
        }
        var streamIds = balances
            .Select(b => b.Id)
            .ToList();

        var eventsQuery = querySession.Events.QueryAllRawEvents()
            .Where(e => streamIds.Contains(e.StreamId)
                        && e.EventTypesAre(typeof(TimeOffBalanceAutomaticallyUpdated), typeof(TimeOffBalanceManuallyUpdated), typeof(TimeOffBalanceRequestChangeEvent)));
        var eventsCount = await eventsQuery.CountAsync(cancellationToken);

            var events = await eventsQuery
                .OrderByDescending(e => e.Sequence)
                .Skip((request.Page - 1) * request.PerPage)
                .Take(request.PerPage)
                .ToListAsync(token: cancellationToken);

        var eventDtos = events.Select(e =>
        {
            var changeType = TimeOffBalancesHistoryItemType.Undefined;
            
            decimal delta = 0;
            Guid? updatedByEmployeeId = null;
            DateTimeOffset timestamp = default;

            if (e.EventType == typeof(TimeOffBalanceAutomaticallyUpdated))
            {
                var eventData = (TimeOffBalanceAutomaticallyUpdated)e.Data;
                delta = eventData.Delta;
                timestamp = eventData.OccurredAt;
                changeType = TimeOffBalancesHistoryItemType.AutoUpdate;
            }
            else if (e.EventType == typeof(TimeOffBalanceManuallyUpdated))
            {
                var eventData = (TimeOffBalanceManuallyUpdated)e.Data;
                delta = eventData.Delta;
                updatedByEmployeeId = eventData.PerformedByEmployeeId;
                timestamp = eventData.OccurredAt;
                changeType = TimeOffBalancesHistoryItemType.ManualAdjustment;
            }
            else if (e.EventType == typeof(TimeOffBalanceRequestChangeEvent))
            {
                var eventData = (TimeOffBalanceRequestChangeEvent)e.Data;
                delta = eventData.Delta;
                timestamp = eventData.OccurredAt;
                changeType = eventData.Delta <= 0 ? TimeOffBalancesHistoryItemType.TimeOffRequest : TimeOffBalancesHistoryItemType.TimeOffRequestCancellationOrRejection;
            }
            
            var balanceId = e.StreamId;
            var balance = balancesDict[balanceId];
            var balanceType = balanceTypes.GetValueOrDefault(balance.TypeId);
            
            return new TimeOffBalancesHistoryItemDto(
                balanceId,
                balanceType?.Id ?? Guid.Empty,
                balanceType?.Name ?? string.Empty,
                balanceType?.Emoji ?? string.Empty,
                timestamp, 
                delta,
                updatedByEmployeeId,
                changeType);
        }).ToList();
        
        var employeesIds = eventDtos
            .Where(e => e.UpdatedByEmployeeId.HasValue)
            .Select(e => e.UpdatedByEmployeeId!.Value)
            .Distinct()
            .ToList();
        var employees = await querySession.Query<Employee>()
            .Where(e => employeesIds.Contains(e.Id))
            .ToListAsync(cancellationToken);
        var employeesDictionary = employees.ToDictionary(e => e.Id, e => e);
        
        foreach (var eventDto in eventDtos)
        {
            if (!eventDto.UpdatedByEmployeeId.HasValue)
            {
                continue;
            }

            var employee = employeesDictionary.GetValueOrDefault(eventDto.UpdatedByEmployeeId.Value);
            if (employee is null)
            {
                continue;
            }
            
            eventDto.UpdatedBy = new EmployeeTitleDto(employee.Id, employee.FirstName, employee.MiddleName, employee.LastName, employee.Email);
        }

        return new PagedDto<TimeOffBalancesHistoryItemDto>(eventDtos, eventsCount);
    }
}
