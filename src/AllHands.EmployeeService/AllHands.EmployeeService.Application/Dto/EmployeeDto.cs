using AllHands.EmployeeService.Domain.Models;

namespace AllHands.EmployeeService.Application.Dto;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } =  string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public PositionDto Position { get; set; } = null!;

    public static EmployeeDto FromModel(Employee model)
    {
        return new EmployeeDto
        {
            Id = model.Id,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Position = model.Position is not null
                ? new PositionDto()
                {
                    Id = model.Position.Id,
                    Name = model.Position.Name
                }
                : null!
        };
    }
}
