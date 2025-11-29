using FluentValidation;

namespace AllHands.Application.Features.News.Update;

public sealed class UpdateNewsPostCommandValidator : AbstractValidator<UpdateNewsPostCommand>
{
    public UpdateNewsPostCommandValidator(NewsCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
