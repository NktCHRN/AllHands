using AllHands.Application.Dto;

namespace AllHands.Application.Abstractions;

public interface IFileService
{
    Task SaveAvatarAsync(AllHandsFile file, CancellationToken cancellationToken);
    Task<AllHandsFile> GetAvatarAsync(string id, CancellationToken cancellationToken);
    Task SaveCompanyLogoAsync(AllHandsFile file, CancellationToken cancellationToken);
    Task<AllHandsFile> GetCompanyLogoAsync(string id, CancellationToken cancellationToken);
}
