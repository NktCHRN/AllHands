using AllHands.Shared.Application.Auth;
using MediatR;

namespace AllHands.AuthService.Application.Features.Permissions.Get;

public sealed class GetPermissionsHandler(IPermissionsContainer permissionsContainer) : IRequestHandler<GetPermissionsQuery, GetPermissionsResult>
{
    public Task<GetPermissionsResult> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = permissionsContainer.Permissions
            .Keys
            .Order()
            .ToList();
        
        return Task.FromResult(new GetPermissionsResult(permissions));
    }
}
