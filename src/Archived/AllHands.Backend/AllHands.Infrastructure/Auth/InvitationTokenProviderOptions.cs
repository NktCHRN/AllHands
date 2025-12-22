using System.ComponentModel.DataAnnotations;

namespace AllHands.Infrastructure.Auth;

public sealed class InvitationTokenProviderOptions
{
    [Range(1, int.MaxValue)]
    public required int LifeTimeInMinutes { get; init; }
    [Range(1, int.MaxValue)]
    public required int TokenRecreationTimeoutInSeconds { get; init; }
    public TimeSpan TokenRecreationTimeout => TimeSpan.FromSeconds(TokenRecreationTimeoutInSeconds);
}
