using MediatR;

namespace AllHands.EmployeeService.Application.Features.Company.UpdateLogo;

public sealed record UpdateLogoCommand(Stream Stream, string Name, string ContentType) : IRequest;
