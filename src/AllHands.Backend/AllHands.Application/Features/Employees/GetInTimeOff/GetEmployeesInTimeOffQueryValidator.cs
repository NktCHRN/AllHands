using FluentValidation;

namespace AllHands.Application.Features.Employees.GetInTimeOff;

public sealed class GetEmployeesInTimeOffQueryValidator : AbstractValidator<GetEmployeesInTimeOffQuery>
{
    private const int MaxRange = 31;
    
    public GetEmployeesInTimeOffQueryValidator()
    {
        RuleFor(x => x.Start)
            .LessThanOrEqualTo(x => x.End);
        
        RuleFor(x => x)
            .Must(x =>
            {
                var totalDaysCount = (x.End.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) -
                                     x.Start.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)).Days + 1;
                return totalDaysCount <= MaxRange;
            })
            .WithMessage($"Max range for this query is {MaxRange} days.");
    }
}
