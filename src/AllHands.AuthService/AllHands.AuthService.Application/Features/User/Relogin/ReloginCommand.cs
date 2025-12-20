using MediatR;

namespace AllHands.AuthService.Application.Features.User.Relogin;

public sealed record ReloginCommand(Guid CompanyId) : IRequest;
