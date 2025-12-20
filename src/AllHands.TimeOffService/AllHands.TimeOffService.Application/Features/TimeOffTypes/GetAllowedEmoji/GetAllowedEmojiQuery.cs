using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.GetAllowedEmoji;

public sealed record GetAllowedEmojiQuery() : IRequest<GetAllowedEmojiResult>;
