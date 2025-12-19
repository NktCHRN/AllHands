using System.Collections;

namespace AllHands.Shared.Domain.UserContext;

public interface IUserContext
{
    public Guid Id { get; }
    public string Email { get; } 
    public string? PhoneNumber { get; }
    public Guid CompanyId { get; }
    public string FirstName { get; }
    public string? MiddleName { get; }
    public string LastName { get; }
    public Guid EmployeeId { get; }

    public IReadOnlyList<string> Roles { get; }
    public BitArray Permissions { get; }
}