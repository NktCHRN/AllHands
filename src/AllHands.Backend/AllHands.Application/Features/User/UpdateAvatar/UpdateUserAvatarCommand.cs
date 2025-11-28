using MediatR;

namespace AllHands.Application.Features.User.UpdateAvatar;

public sealed record UpdateUserAvatarCommand(Stream Stream, string Name, string ContentType) : IRequest;
