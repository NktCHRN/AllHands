using AllHands.Domain.Models;

namespace AllHands.Application.Dto;

public sealed record EmployeeDetailsDto(
    Guid EmployeeId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    string? PhoneNumber,
    EmployeeStatus Status,
    DateOnly WorkStartDate,
    EmployeeDto Manager,
    PositionDto Position,
    CompanyDto Company,
    RoleDto? Role)
{
    public static EmployeeDetailsDto FromModel(Employee model, RoleDto? role)
    {
        return new EmployeeDetailsDto(
            model.Id,
            model.FirstName,
            model.MiddleName,
            model.LastName,
            model.Email,
            model.PhoneNumber,
            model.Status,
            model.WorkStartDate,
            model.Manager is not null ? EmployeeDto.FromModel(model.Manager) : null!,
            model.Position is not null 
                ? new PositionDto
                {
                    Id = model.Position.Id, 
                    Name = model.Position.Name
                }
                : null!,
            model.Company is not null
                ? new CompanyDto()
                {
                    Id = model.Company.Id,
                    Name = model.Company.Name
                }
                : null!,
            role);
    }
}
    