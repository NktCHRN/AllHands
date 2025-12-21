namespace AllHands.Shared.Infrastructure.GrpcInfrastructure;

public class GrpcRetryOptions
{
    public int? MaxAttempts { get; set; } = 3;

    public TimeSpan? InitialBackoff { get; set; } = TimeSpan.FromSeconds(1);

    public TimeSpan? MaxBackoff{ get; set; } = TimeSpan.FromSeconds(5);

    public double? BackoffMultiplier { get; set; } = 1.5;
}
