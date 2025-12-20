using AllHands.AuthService.Domain.Models;

namespace AllHands.AuthService.Application.Dto;

public class EmployeeTitleDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } =  string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    
    public static EmployeeTitleDto FromModel(AllHandsIdentityUser model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new EmployeeTitleDto()
            {
                UserId = model.Id,
                Id = model.EmployeeId, 
                FirstName = model.FirstName, 
                MiddleName = model.MiddleName, 
                LastName = model.LastName, 
                Email = model.Email ?? string.Empty
            };
    }
}
