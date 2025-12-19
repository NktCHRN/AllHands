namespace AllHands.Shared.Domain.UserContext;

public sealed class UserContextService(IServiceProvider serviceProvider) : IUserContextAccessor, IUserContextSetuper
{
    public IUserContext? UserContext => LocalUserContext.Value;
    
    private static readonly AsyncLocal<UserContext?> LocalUserContext = new AsyncLocal<UserContext?>();
    
    public void Push(UserContext userContext)
    {
        LocalUserContext.Value = userContext;
    }
}
