using AllHands.TimeOffBalanceAutoUpdater.Events.TimeOffBalance;
using AllHands.TimeOffBalanceAutoUpdater.Models;
using Marten;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllHands.TimeOffBalanceAutoUpdater;

public sealed class TimeOffService(IDocumentStore documentStore, IOptions<TimeOffBalanceAutoUpdaterOptions> options, ILogger<TimeOffService> logger)
{
    private long _processedCount;
    private long _updatedCount;
    
    public async Task<TimeOffBalancesUpdateResult> UpdateAllAsync(CancellationToken cancellationToken)
    {
        var hasErrors = false;
        _processedCount = 0;
        _updatedCount = 0;
        
        await using var session = documentStore.QuerySession();
                
        var companyIds = await session.Query<Company>()
            .Where(x => x.AnyTenant())
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var companyId in companyIds)
        {
            try
            {
                logger.LogInformation("Processing company {CompanyId}", companyId);
                await UpdateInCompanyAsync(session, companyId, cancellationToken);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationToken ||
                                                        ex.CancellationToken.IsCancellationRequested)
            {
                hasErrors = true;
                break;
            }
            catch (Exception e)
            {
                hasErrors = true;
                logger.LogError(e, "Unhandled exception in SessionRecalculatorBackgroundService");
            }
        }
        
        return new TimeOffBalancesUpdateResult(_processedCount, _updatedCount, hasErrors);
    }

    private async Task UpdateInCompanyAsync(IQuerySession querySession, Guid companyId, CancellationToken cancellationToken)
    {
        var company = await querySession.Query<Company>().
                          FirstOrDefaultAsync(x => x.AnyTenant() && x.Id == companyId, cancellationToken)
                      ?? throw new InvalidOperationException("Company was not found.");
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
            .Where(x => x.TenantIsOneOf(company.Id.ToString()) && x.WorkStartDate < currentMonthStart && x.Status != EmployeeStatus.Fired)
            .CountAsync(cancellationToken);

        var batchSize = options.Value.BatchSize;
        for (var skip = 0; skip < employeesCount; skip += batchSize)
        {
            await using var documentSession = documentStore.LightweightSession(company.Id.ToString());
            var employees = await documentSession.Query<Employee>()
                .Where(x => x.WorkStartDate < currentMonthStart && x.Status != EmployeeStatus.Fired)
                .OrderBy(x => x.Id)
                .Skip(skip)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            foreach (var employee in employees)
            {
                var balances = await documentSession.Query<TimeOffBalance>()
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
                    _updatedCount++;
                }

                foreach (var timeOffType in timeOffTypes)
                {
                    if (balances.Any(b => b.TypeId == timeOffType.Id) || timeOffType.DaysPerYear <= 0)
                    {
                        continue;
                    }

                    var balanceId = TimeOffBalance.CreateId(employee.Id, timeOffType.Id);
                    var amount = GetAmount(timeOffType.DaysPerYear, daysInPreviousMonth, employee.WorkStartDate, currentMonthStart);
                    documentSession.Events.StartStream<TimeOffBalance>(
                        balanceId, 
                        new TimeOffBalanceCreatedEvent(balanceId, employee.Id, timeOffType.Id, timeOffType.DaysPerYear),
                        new TimeOffBalanceAutomaticallyUpdated(balanceId, amount));
                    _updatedCount++;
                }
            }
            
            await documentSession.SaveChangesAsync(cancellationToken);
            _processedCount += employees.Count;
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
