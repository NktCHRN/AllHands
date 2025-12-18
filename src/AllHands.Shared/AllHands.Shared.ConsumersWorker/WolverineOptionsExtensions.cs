using Wolverine;

namespace AllHands.Shared.ConsumersWorker;

public static class WolverineOptionsExtensions
{
    public static void AddIncomingHeadersMiddleware(this WolverineOptions wolverineOptions)
    {
        wolverineOptions.Policies.AddMiddleware<UserContextHeadersWolverineMiddleware>();
    }
}
