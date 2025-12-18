namespace AllHands.Shared.Domain.UserContext;

public sealed class UserContextAccessor(IServiceProvider serviceProvider) : IUserContextAccessor
{
    public UserContext? UserContext => serviceProvider.GetService(typeof(UserContext)) as UserContext;
}
