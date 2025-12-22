using AllHands.Application.Validation;
using FluentValidation;

namespace AllHands.Application.Features.Roles.GetUsersInRole;

public sealed class GetUsersInRoleQueryValidator : AbstractValidator<GetUsersInRoleQuery>
{
    public GetUsersInRoleQueryValidator(BasePagedQueryValidator baseValidator)
    {
        Include(baseValidator);
    }
}
