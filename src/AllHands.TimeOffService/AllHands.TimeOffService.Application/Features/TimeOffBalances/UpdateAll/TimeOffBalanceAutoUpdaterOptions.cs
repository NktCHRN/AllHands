using System.ComponentModel.DataAnnotations;

namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.UpdateAll;

public class TimeOffBalanceAutoUpdaterOptions
{
    [Range(1, int.MaxValue)] 
    public int BatchSize { get; set; } = 1000;
}
