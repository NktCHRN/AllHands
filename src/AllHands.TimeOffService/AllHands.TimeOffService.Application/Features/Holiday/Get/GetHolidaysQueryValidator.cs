using FluentValidation;

namespace AllHands.TimeOffService.Application.Features.Holiday.Get;

public sealed class GetHolidaysQueryValidator : AbstractValidator<GetHolidaysQuery>
{
    public GetHolidaysQueryValidator()
    {
        RuleFor(x => x.Start)
            .LessThanOrEqualTo(x => x.End);
        
        RuleFor(x => x.Start)
            .GreaterThanOrEqualTo(x => DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3)));
        
        RuleFor(x => x.End)
            .LessThanOrEqualTo(x => DateOnly.FromDateTime(DateTime.UtcNow.AddYears(3)));
    }
}
