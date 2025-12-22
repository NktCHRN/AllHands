namespace AllHands.Application.Abstractions;

public interface IPermissionsContainer
{
    IReadOnlyDictionary<string, int> Permissions { get; }
    int PermissionsLength { get; }
}
