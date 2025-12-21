using Grpc.Core;
using Grpc.Net.Client.Configuration;

namespace AllHands.Shared.Infrastructure.GrpcInfrastructure;

public static class GrpcRetryProvider
{
    public static RetryPolicy GetDefaultRetryPolicy(GrpcRetryOptions options)
    {
        return new RetryPolicy
        {
            MaxAttempts = options.MaxAttempts,
            InitialBackoff = options.InitialBackoff,
            MaxBackoff = options.MaxBackoff,
            BackoffMultiplier = options.BackoffMultiplier,
            RetryableStatusCodes =
            {
                StatusCode.Unavailable,
                StatusCode.ResourceExhausted,
                StatusCode.Aborted,
                StatusCode.DeadlineExceeded
            }
        };
    }
}
