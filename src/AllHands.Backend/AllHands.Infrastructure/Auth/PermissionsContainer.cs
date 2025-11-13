using System.Collections.Frozen;
using AllHands.Infrastructure.Abstractions;

namespace AllHands.Infrastructure.Auth;

public sealed class PermissionsContainer : IPermissionsContainer
{
    public IReadOnlyDictionary<string, int> Permissions { get; } = new Dictionary<string, int>()
        {
            {"roles.create", 1},
            {"roles.edit", 2},
            {"roles.delete", 3},
            {"company.edit", 4},
        }
        .ToFrozenDictionary();
}
