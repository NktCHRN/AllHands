using AllHands.TimeOffService.Domain.Models;

namespace AllHands.TimeOffService.Domain.Abstractions;

public interface IWorkDaysCalculator
{
    public int Calculate(DateOnly start, DateOnly end, Company company, IEnumerable<Holiday> holidays);
}