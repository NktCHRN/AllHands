using FluentValidation;

namespace AllHands.NewsService.Application.Features.News.Create;

public sealed class CreateNewsPostCommandValidator : AbstractValidator<CreateNewsPostCommand>
{
    public CreateNewsPostCommandValidator(NewsCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
