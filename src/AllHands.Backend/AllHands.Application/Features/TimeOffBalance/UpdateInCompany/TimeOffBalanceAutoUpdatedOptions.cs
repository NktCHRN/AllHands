using System.ComponentModel.DataAnnotations;

namespace AllHands.Application.Features.TimeOffBalance.UpdateInCompany;

public class TimeOffBalanceAutoUpdaterOptions
{
    [Required] 
    [Range(0, int.MaxValue)] 
    public int BatchSize { get; set; } = 1000;
    [Required]
    public TimeSpan Timeout { get; set; } = TimeSpan.FromDays(1);
}