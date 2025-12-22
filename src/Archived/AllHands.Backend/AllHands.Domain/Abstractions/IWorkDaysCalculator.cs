using AllHands.Domain.Models;

namespace AllHands.Domain.Abstractions;

public interface IWorkDaysCalculator
{
    public int Calculate(DateOnly start, DateOnly end, Company company, IEnumerable<Holiday> holidays);
}