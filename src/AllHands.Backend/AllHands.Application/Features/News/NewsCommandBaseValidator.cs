using FluentValidation;

namespace AllHands.Application.Features.News;

public sealed class NewsCommandBaseValidator : AbstractValidator<NewsCommandBase>
{
    public NewsCommandBaseValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
