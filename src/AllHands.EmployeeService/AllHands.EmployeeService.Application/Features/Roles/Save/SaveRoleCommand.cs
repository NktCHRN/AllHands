using MediatR;

namespace AllHands.EmployeeService.Application.Features.Roles.Save;

public record SaveRoleCommand(Guid Id, string Name, bool IsDefault, Guid CompanyId) : IRequest;
