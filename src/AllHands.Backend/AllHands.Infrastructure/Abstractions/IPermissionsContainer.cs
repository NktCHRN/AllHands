namespace AllHands.Infrastructure.Abstractions;

public interface IPermissionsContainer
{
    IReadOnlyDictionary<string, int> Permissions { get; }
}