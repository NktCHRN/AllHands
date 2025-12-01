using MediatR;

namespace AllHands.Application.Features.Employees.GetAvatarById;

public sealed record GetAvatarByIdQuery(Guid EmployeeId) : IRequest<GetAvatarByIdResult>;
