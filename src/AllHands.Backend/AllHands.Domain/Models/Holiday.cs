using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Models;

public sealed class Holiday : IIdentifiable
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid CompanyId { get; set; }
    public required DateOnly Date { get; set; }
}
