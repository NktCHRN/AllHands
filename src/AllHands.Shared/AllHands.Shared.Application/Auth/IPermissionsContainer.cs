namespace AllHands.Shared.Application.Auth;

public interface IPermissionsContainer
{
    IReadOnlyDictionary<string, int> Permissions { get; }
    int BitArrayLength { get; }
}
