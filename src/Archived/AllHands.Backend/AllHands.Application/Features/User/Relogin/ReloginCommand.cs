using MediatR;

namespace AllHands.Application.Features.User.Relogin;

public sealed record ReloginCommand(Guid CompanyId) : IRequest;
