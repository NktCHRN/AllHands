using MediatR;

namespace AllHands.AuthService.Application.Features.Permissions.Get;

public sealed record GetPermissionsQuery() : IRequest<GetPermissionsResult>;