using System.ComponentModel.DataAnnotations;

namespace AllHands.AuthService.Infrastructure.Auth;

public sealed class PasswordResetTokenProviderOptions
{
    [Required]
    public TimeSpan LifeTime { get; set; }
    [Required]
    public TimeSpan TokenRecreationTimeout { get; set; }
}