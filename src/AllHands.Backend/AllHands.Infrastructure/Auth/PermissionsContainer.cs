using System.Collections.Frozen;
using AllHands.Infrastructure.Abstractions;

namespace AllHands.Infrastructure.Auth;

public sealed class PermissionsContainer : IPermissionsContainer
{
    public IReadOnlyDictionary<string, int> Permissions { get; } = new Dictionary<string, int>()
        {
            {"roles.create", 1 },
            {"roles.edit", 2 },
            {"roles.delete", 3 },
            {"company.edit", 4 },
            {"employee.create", 5 },
            {"employee.edit", 6 },
            {"employee.delete", 7},
            {"timeoffrequest.adminapprove", 8 },
            {"company.statistics.view", 9},
            {"roles.view", 10 },
            {"news.create", 11 },
            {"news.edit", 12 },
            {"news.delete", 13 },
            {"employee.rehire", 14 },
            {"timeofftype.create", 15 },
            {"timeofftype.edit", 16 },
            {"timeofftype.delete", 17},
            {"timeoffbalance.edit", 18 },
            {"holiday.create", 19 },
            {"holiday.edit", 20 },
            {"holiday.delete", 21},
            {"position.create", 22 },
            {"position.edit", 23 },
            {"position.delete", 24},
        }
        .ToFrozenDictionary();
}
