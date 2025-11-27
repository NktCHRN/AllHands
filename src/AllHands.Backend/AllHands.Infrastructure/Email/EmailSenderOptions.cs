using System.ComponentModel.DataAnnotations;

namespace AllHands.Infrastructure.Email;

public sealed class EmailSenderOptions
{
    [Required]
    public string FromEmailAddress  { get; set; } = string.Empty;
    [Required]
    public string ResetPasswordUrl { get; set; } = string.Empty;
    [Required]
    public string ResetPasswordTemplateName { get; set; } = string.Empty;
    [Required]
    public string CompleteRegistrationUrl { get; set; } = string.Empty;
    [Required]
    public string CompleteRegistrationTemplateName { get; set; } = string.Empty;
}
