using System.Collections;

namespace AllHands.Shared.Domain.UserContext;

public sealed class UserContext
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Guid CompanyId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public Guid EmployeeId { get; set; }

    public IReadOnlyList<string> Roles { get; set; } = [];
    public BitArray Permissions { get; set; } = new BitArray(0);
}
