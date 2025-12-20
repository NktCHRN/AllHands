using AllHands.Shared.Application.Dto;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Create;

public sealed record CreateTimeOffRequestCommand(
    Guid TypeId,
    DateOnly StartDate,
    DateOnly EndDate) : IRequest<CreatedEntityDto>;
