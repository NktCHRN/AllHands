using FluentValidation;

namespace AllHands.Application.Features.Roles.Update;

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator(RoleCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
