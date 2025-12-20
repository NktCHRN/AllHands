using MediatR;

namespace AllHands.TimeOffService.Application.Features.Holiday.Delete;

public sealed record DeleteHolidayCommand(Guid Id) : IRequest;
