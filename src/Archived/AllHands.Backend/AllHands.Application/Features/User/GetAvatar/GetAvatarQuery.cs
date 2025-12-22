using MediatR;

namespace AllHands.Application.Features.User.GetAvatar;

public sealed record GetAvatarQuery() : IRequest<GetAvatarResult>;
