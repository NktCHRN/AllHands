using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.DeleteAvatar;

public sealed record DeleteAvatarByIdCommand(Guid EmployeeId) : IRequest;
