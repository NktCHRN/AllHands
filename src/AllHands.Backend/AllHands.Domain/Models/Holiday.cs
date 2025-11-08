namespace AllHands.Domain.Models;

public sealed class Holiday
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid CompanyId { get; set; }
    public required DateOnly Date { get; set; }
}
