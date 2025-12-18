namespace AllHands.Shared.Domain.UserContext;

public interface IUserContextAccessor
{
    UserContext? UserContext { get; }
}