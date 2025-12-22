using AllHands.AuthService.Contracts.Protos.Grpc;
using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Application.Features.Employees.Create;
using AllHands.EmployeeService.Application.Features.Employees.Update;

namespace AllHands.EmployeeService.Infrastructure.Clients;

public sealed class IdentityUserClient(UserService.UserServiceClient client) : IUserClient
{
    public async Task<CreateIdentityUserResult> CreateAsync(CreateIdentityUserCommand command, CancellationToken cancellationToken)
    {
        var result = await client.CreateAsync(new CreateUserRequest()
        {
            Email = command.Email,
            EmployeeId = command.EmployeeId.ToString(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            MiddleName = command.MiddleName,
            PhoneNumber = command.PhoneNumber,
        }, cancellationToken: cancellationToken);
        
        return new CreateIdentityUserResult(Guid.Parse(result.UserId), Guid.Parse(result.RoleId), Guid.Parse(result.GlobalUserId));
    }

    public async Task<UpdateIdentityUserResult> UpdateAsync(UpdateIdentityUserCommand command, CancellationToken cancellationToken)
    {
        var result = await client.UpdateAsync(new UpdateUserRequest()
        {
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            MiddleName = command.MiddleName,
            PhoneNumber = command.PhoneNumber,
            UserId = command.UserId.ToString(),
            RoleId = command.RoleId?.ToString(),
        }, cancellationToken: cancellationToken);
        
        return new UpdateIdentityUserResult(Guid.Parse(result.UserId), Guid.Parse(result.RoleId), Guid.Parse(result.GlobalUserId));
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        await client.DeleteAsync(new DeleteUserRequest()
        {
            UserId = userId.ToString(),
        }, cancellationToken: cancellationToken);
    }
}
