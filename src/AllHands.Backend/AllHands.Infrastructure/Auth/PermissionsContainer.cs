using System.Collections.Frozen;

namespace AllHands.Infrastructure.Auth;

public sealed class PermissionsContainer
{
    public readonly FrozenDictionary<string, int> Dictionary = new Dictionary<string, int>()
        {
            {"roles.create", 1},
            {"roles.edit", 2},
            {"roles.delete", 3},
            {"company.edit", 4},
        }
        .ToFrozenDictionary();
}
