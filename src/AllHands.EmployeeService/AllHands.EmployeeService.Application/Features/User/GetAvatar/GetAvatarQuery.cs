using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.GetAvatar;

public sealed record GetAvatarQuery() : IRequest<GetAvatarResult>;
