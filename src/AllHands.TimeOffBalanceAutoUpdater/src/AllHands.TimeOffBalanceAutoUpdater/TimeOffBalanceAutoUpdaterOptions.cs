using System.ComponentModel.DataAnnotations;

namespace AllHands.TimeOffBalanceAutoUpdater;

public class TimeOffBalanceAutoUpdaterOptions
{
    [Range(1, int.MaxValue)] 
    public int BatchSize { get; set; } = 1000;
}
