using FluentValidation;

namespace AllHands.NewsService.Application.Features.News.Update;

public sealed class UpdateNewsPostCommandValidator : AbstractValidator<UpdateNewsPostCommand>
{
    public UpdateNewsPostCommandValidator(NewsCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
