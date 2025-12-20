using MediatR;

namespace AllHands.TimeOffService.Application.Features.Companies.Delete;

public record DeleteCompanyCommand(Guid Id) : IRequest;
