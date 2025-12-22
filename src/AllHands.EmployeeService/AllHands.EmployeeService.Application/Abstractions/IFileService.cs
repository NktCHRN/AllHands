using AllHands.EmployeeService.Application.Dto;

namespace AllHands.EmployeeService.Application.Abstractions;

public interface IFileService
{
    Task SaveAvatarAsync(AllHandsFile file, CancellationToken cancellationToken);
    Task<AllHandsFile> GetAvatarAsync(string id, CancellationToken cancellationToken);
    Task SaveCompanyLogoAsync(AllHandsFile file, CancellationToken cancellationToken);
    Task<AllHandsFile> GetCompanyLogoAsync(string id, CancellationToken cancellationToken);
    Task DeleteAvatarAsync(string id, CancellationToken cancellationToken);
    Task DeleteCompanyLogoAsync(string id, CancellationToken cancellationToken);
}
