using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.GetAvatarById;

public sealed record GetAvatarByIdQuery(Guid EmployeeId) : IRequest<GetAvatarByIdResult>;
