using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.Options;

namespace AllHands.Application.Features.TimeOffBalance.UpdateInCompany;

public sealed class UpdateTimeOffBalanceInCompanyHandler(IDocumentStore documentStore, TimeProvider timeProvider, IOptions<TimeOffBalanceAutoUpdaterOptions> options) : IRequestHandler<UpdateTimeOffBalanceInCompanyCommand>
{
    public async Task Handle(UpdateTimeOffBalanceInCompanyCommand request, CancellationToken cancellationToken)
    {
        await using var querySession = documentStore.QuerySession();
        var company = await querySession.Query<Domain.Models.Company>().
                          FirstOrDefaultAsync(x => x.AnyTenant() && x.Id == request.CompanyId, cancellationToken)
                      ?? throw new EntityNotFoundException("Company was not found.");
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(company.IanaTimeZone);
        var timeOffTypes = await querySession.Query<TimeOffType>()
            .Where(x => x.TenantIsOneOf(company.Id.ToString()))
            .ToListAsync(cancellationToken);
        var timeOffTypesIds = timeOffTypes.Select(x => x.Id).ToList();

        var currentInZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        var currentMonthStart = new DateOnly(currentInZone.Year, currentInZone.Month, 1);
        var currentMonthStartDateTime = TimeZoneInfo.ConvertTimeToUtc(currentMonthStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified), timeZone);
        var daysInPreviousMonth = DateTime.DaysInMonth(currentInZone.Year, currentInZone.Month - 1);
        var employeesCount = await querySession.Query<Employee>()
            .Where(x => x.TenantIsOneOf(company.Id.ToString()) && x.WorkStartDate < currentMonthStart)
            .CountAsync(cancellationToken);

        var batchSize = options.Value.BatchSize;
        for (var i = 0; i < employeesCount; i += batchSize)
        {
            await using var documentSession = documentStore.LightweightSession(company.Id.ToString());
            var employees = await documentSession.Query<Employee>()
                .Where(x => x.CompanyId == request.CompanyId && x.WorkStartDate < currentMonthStart)
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);

            foreach (var employee in employees)
            {
                var balances = await documentSession.Query<Domain.Models.TimeOffBalance>()
                    .Where(x => x.EmployeeId == employee.Id && timeOffTypesIds.Contains(x.TypeId))
                    .ToListAsync(cancellationToken);
                foreach (var balance in balances)
                {
                    if (balance.LastAutoUpdate >= currentMonthStartDateTime || balance.DaysPerYear <= 0)
                    {
                        continue;
                    }

                    var amount = GetAmount(balance.DaysPerYear, daysInPreviousMonth, employee.WorkStartDate, currentMonthStart);
                    documentSession.Events.Append(balance.Id, new TimeOffBalanceAutomaticallyUpdated(balance.Id, amount));
                }

                foreach (var timeOffType in timeOffTypes)
                {
                    if (balances.Any(b => b.TypeId == timeOffType.Id) || timeOffType.DaysPerYear <= 0)
                    {
                        continue;
                    }

                    var balanceId = Domain.Models.TimeOffBalance.CreateId(employee.Id, timeOffType.Id);
                    var amount = GetAmount(timeOffType.DaysPerYear, daysInPreviousMonth, employee.WorkStartDate, currentMonthStart);
                    documentSession.Events.StartStream<Domain.Models.TimeOffBalance>(
                        balanceId, 
                        new TimeOffBalanceCreatedEvent(balanceId, employee.Id, timeOffType.Id, timeOffType.DaysPerYear),
                        new TimeOffBalanceAutomaticallyUpdated(balanceId, amount));
                }
            }
            
            await documentSession.SaveChangesAsync(cancellationToken);
        }
    }

    private static decimal GetAmount(decimal amountPerYear, int daysInMonth, DateOnly workStartDate, DateOnly endDate)
    {
        var previousMonthStartDate = endDate.AddMonths(-1);
        var startDate = workStartDate > previousMonthStartDate ? workStartDate : previousMonthStartDate;
        var amountPerDay = amountPerYear / 12.0m / daysInMonth;
        var days = endDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) - startDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        return amountPerDay * days.Days;
    }
}
