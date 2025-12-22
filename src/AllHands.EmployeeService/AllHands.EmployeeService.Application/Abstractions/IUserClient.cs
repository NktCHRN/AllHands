using AllHands.EmployeeService.Application.Features.Employees.Create;
using AllHands.EmployeeService.Application.Features.Employees.Update;

namespace AllHands.EmployeeService.Application.Abstractions;

public interface IUserClient
{
    Task<CreateIdentityUserResult> CreateAsync(CreateIdentityUserCommand command, CancellationToken cancellationToken);
    Task<UpdateIdentityUserResult> UpdateAsync(UpdateIdentityUserCommand command, CancellationToken cancellationToken);
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken);
}
