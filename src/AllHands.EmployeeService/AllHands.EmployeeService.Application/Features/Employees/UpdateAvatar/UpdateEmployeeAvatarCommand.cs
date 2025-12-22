using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.UpdateAvatar;

public sealed record UpdateEmployeeAvatarCommand(Guid EmployeeId, Stream Stream, string Name, string ContentType) : IRequest;
