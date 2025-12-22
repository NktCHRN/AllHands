using MediatR;

namespace AllHands.Application.Features.TimeOffTypes.GetAllowedEmoji;

public sealed record GetAllowedEmojiQuery() : IRequest<GetAllowedEmojiResult>;
