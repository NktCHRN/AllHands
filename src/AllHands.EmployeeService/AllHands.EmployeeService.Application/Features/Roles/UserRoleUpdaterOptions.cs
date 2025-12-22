using System.ComponentModel.DataAnnotations;

namespace AllHands.EmployeeService.Application.Features.Roles;

public class UserRoleUpdaterOptions
{
    [Range(1, int.MaxValue)]
    public int BatchSize { get; set; } = 1000;
}
