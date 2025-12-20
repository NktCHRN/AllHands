using System.ComponentModel.DataAnnotations;

namespace AllHands.AuthService.SessionRecalculator;

public sealed class SessionRecalculatorOptions
{
    [Required] public int BatchSize { get; set; } = 1000;
}