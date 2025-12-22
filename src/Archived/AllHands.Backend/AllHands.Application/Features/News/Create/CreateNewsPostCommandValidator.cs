using FluentValidation;

namespace AllHands.Application.Features.News.Create;

public sealed class CreateNewsPostCommandValidator : AbstractValidator<CreateNewsPostCommand>
{
    public CreateNewsPostCommandValidator(NewsCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
