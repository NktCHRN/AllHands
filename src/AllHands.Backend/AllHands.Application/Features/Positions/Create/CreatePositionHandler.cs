using AllHands.Application.Abstractions;
using MediatR;

namespace AllHands.Application.Features.Positions.Create;

public sealed class CreatePositionHandler(ICurrentUserService currentUserService) : IRequestHandler<CreatePositionCommand>
{
    public Task Handle(CreatePositionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
