using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Create;

public sealed record CreateTimeOffRequestCommand(
    Guid TypeId,
    DateOnly StartDate,
    DateOnly EndDate) : IRequest<CreatedEntityDto>;
