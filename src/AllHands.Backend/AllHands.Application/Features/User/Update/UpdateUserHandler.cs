using MediatR;

namespace AllHands.Application.Features.User.Update;

public sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
