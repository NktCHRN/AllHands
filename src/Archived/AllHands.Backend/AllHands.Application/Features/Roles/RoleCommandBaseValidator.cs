using AllHands.Application.Abstractions;
using FluentValidation;

namespace AllHands.Application.Features.Roles;

public sealed class RoleCommandBaseValidator : AbstractValidator<RoleCommandBase>
{
    public RoleCommandBaseValidator(IPermissionsContainer permissionsContainer)
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(255);
        RuleFor(command => command.Permissions)
            .NotNull()
            .Must((_, p, context) =>
            {
                foreach (var permission in p)
                {
                    if (!permissionsContainer.Permissions.ContainsKey(permission))
                    {
                        context.MessageFormatter.AppendArgument("PermissionName", permission);
                        return false;
                    }
                }
                return true;
            })
            .WithMessage("Permission {PermissionName} was not found");
    }
}
