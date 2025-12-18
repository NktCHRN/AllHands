using AllHands.NewsService.Domain.Models;

namespace AllHands.NewsService.Application.Dto;

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
