using MediatR;

namespace AllHands.Application.Features.TimeOffBalances.Update;

public sealed record UpdateBalanceCommand(
    decimal Delta,
    decimal? DaysPerYear,
    string Reason) : IRequest
{
    public Guid EmployeeId { get; set; }
    public Guid TypeId { get; set; }
}
