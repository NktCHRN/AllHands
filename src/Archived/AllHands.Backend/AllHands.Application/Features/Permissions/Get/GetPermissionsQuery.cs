using MediatR;

namespace AllHands.Application.Features.Permissions.Get;

public sealed record GetPermissionsQuery() : IRequest<GetPermissionsResult>;