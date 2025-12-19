namespace AllHands.Shared.Domain.UserContext;

public interface IUserContextAccessor
{
    IUserContext? UserContext { get; }
}