using AllHands.Application.Abstractions;
using MediatR;

namespace AllHands.Application.Features.TimeOffTypes.GetAllowedEmoji;

public sealed class GetAllowedEmojiHandler(ITimeOffEmojiValidator emojiValidator) : IRequestHandler<GetAllowedEmojiQuery, GetAllowedEmojiResult>
{
    public Task<GetAllowedEmojiResult> Handle(GetAllowedEmojiQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetAllowedEmojiResult(emojiValidator.AllowedEmoji));
    }
}
