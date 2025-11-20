using System.ComponentModel.DataAnnotations;

namespace AllHands.Infrastructure.Auth;

public sealed class InvitationTokenProviderOptions
{
    [Range(1, int.MaxValue)]
    public required int LifeTimeInMinutes { get; init; }
}
