using MediatR;

namespace AllHands.Application.Features.Holiday.Delete;

public sealed record DeleteHolidayCommand(Guid Id) : IRequest;
