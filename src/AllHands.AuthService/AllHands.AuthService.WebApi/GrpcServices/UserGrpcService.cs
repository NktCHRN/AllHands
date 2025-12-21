using AllHands.AuthService.Application.Abstractions;
using AllHands.AuthService.Application.Features.User.Create;
using AllHands.AuthService.Application.Features.User.Update;
using AllHands.AuthService.Contracts.Protos.Grpc;
using Grpc.Core;

namespace AllHands.AuthService.WebApi.GrpcServices;

public sealed class UserGrpcService(IAccountService accountService) : UserService.UserServiceBase
{
    public override async Task<CreateUserResponse> Create(CreateUserRequest request, ServerCallContext context)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            Guid.Parse(request.EmployeeId));
        
        var result = await accountService.CreateAsync(command, context.CancellationToken);

        return new CreateUserResponse()
        {
            UserId = result.Id.ToString(),
            RoleId = result.RoleId.ToString(),
            GlobalUserId = result.GlobalUserId.ToString()
        };
    }

    public override async Task<UpdateUserResponse> Update(UpdateUserRequest request, ServerCallContext context)
    {
        var command = new UpdateUserCommand(
            Guid.Parse(request.UserId),
            request.Email,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            Guid.Parse(request.RoleId));
        
        var result = await accountService.UpdateAsync(command, context.CancellationToken);

        return new UpdateUserResponse()
        {
            UserId = result.Id.ToString(),
            RoleId = result.RoleId.ToString(),
            GlobalUserId = result.GlobalUserId.ToString()
        };
    }

    public override async Task<DeleteUserResponse> Delete(DeleteUserRequest request, ServerCallContext context)
    {
        await accountService.DeleteAsync(Guid.Parse(request.UserId), context.CancellationToken);
        
        return new DeleteUserResponse();
    }
}
