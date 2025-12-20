using FluentValidation;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Create;

public sealed class CreateTimeOffRequestCommandValidator : AbstractValidator<CreateTimeOffRequestCommand>
{
    public CreateTimeOffRequestCommandValidator()
    {
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate);
        
        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(x => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)));
        
        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(x => DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));
    }
}
