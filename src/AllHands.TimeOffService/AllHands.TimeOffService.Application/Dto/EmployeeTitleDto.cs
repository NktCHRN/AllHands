using AllHands.TimeOffService.Domain.Models;

namespace AllHands.TimeOffService.Application.Dto;

public sealed record EmployeeTitleDto(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email)
{
    public static EmployeeTitleDto FromModel(Employee model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new EmployeeTitleDto(model.Id, model.FirstName, model.MiddleName, model.LastName, model.Email);
    }
}
