using MediatR;

namespace AllHands.Application.Features.News.Update;

public sealed record UpdateNewsPostCommand(string Text) : NewsCommandBase(Text), IRequest
{
    public Guid Id { get; set; }
}
