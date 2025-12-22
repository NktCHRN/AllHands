using MediatR;

namespace AllHands.Application.Features.Employees.DeleteAvatar;

public sealed record DeleteAvatarByIdCommand(Guid EmployeeId) : IRequest;
