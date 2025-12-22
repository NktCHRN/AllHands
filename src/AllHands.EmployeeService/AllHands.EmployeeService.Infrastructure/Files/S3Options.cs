using System.ComponentModel.DataAnnotations;

namespace AllHands.EmployeeService.Infrastructure.Files;

public sealed class S3Options
{
    [Required]
    public string BucketName { get; set; } = string.Empty;
    [Required]
    public string AvatarsPrefix { get; set; } = string.Empty;
    [Required]
    public string DefaultAvatarName { get; set; } = string.Empty;
    [Required]
    public string CompanyLogosPrefix { get; set; } = string.Empty;
    [Required]
    public string DefaultCompanyLogoName { get; set; } = string.Empty;
}
