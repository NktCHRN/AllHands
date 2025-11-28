using System.ComponentModel.DataAnnotations;

namespace AllHands.Infrastructure.Auth;

public sealed class SessionRecalculatorBackgroundServiceOptions
{
    [Required]
    public TimeSpan PollTimeout { get; set; }
    [Required]
    public int MaxFailedAttempts {get; set;}

    [Required] public int BatchSize { get; set; } = 1000;
}