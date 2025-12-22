using AllHands.Domain.Abstractions;
using AllHands.Domain.Models;

namespace AllHands.Domain.Services;

public sealed class WorkDaysCalculator : IWorkDaysCalculator
{
    public int Calculate(DateOnly start, DateOnly end, Company company, IEnumerable<Holiday> holidays)
    {
        var isHolidayDictionary = holidays.GroupBy(h => h.Date)
            .ToDictionary(h => h.Key, h => h.Any());

        var workDays = 0;
        var current = start;
        while (current <= end)
        {
            if (company.WorkDays.Contains(current.DayOfWeek) && !isHolidayDictionary.ContainsKey(current))
            {
                workDays++;
            }
            current = current.AddDays(1);
        }
        
        return workDays;
    }
}
