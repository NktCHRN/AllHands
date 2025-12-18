using System.Collections.Frozen;
using AllHands.Shared.Application.Auth;

namespace AllHands.Shared.Infrastructure.Auth;

public sealed class PermissionsContainer : IPermissionsContainer
{
    public IReadOnlyDictionary<string, int> Permissions { get; } = new Dictionary<string, int>()
        {
            { Application.Auth.Permissions.RolesCreate, 1 },
            { Application.Auth.Permissions.RolesEdit, 2 },
            { Application.Auth.Permissions.RolesDelete, 3 },

            { Application.Auth.Permissions.CompanyEdit, 4 },

            { Application.Auth.Permissions.EmployeeCreate, 5 },
            { Application.Auth.Permissions.EmployeeEdit, 6 },
            { Application.Auth.Permissions.EmployeeDelete, 7 },

            { Application.Auth.Permissions.TimeOffRequestAdminApprove, 8 },

            { Application.Auth.Permissions.CompanyStatisticsView, 9 },
            { Application.Auth.Permissions.RolesView, 10 },

            { Application.Auth.Permissions.NewsCreate, 11 },
            { Application.Auth.Permissions.NewsEdit, 12 },
            { Application.Auth.Permissions.NewsDelete, 13 },
            
            // 14 is empty for now.

            { Application.Auth.Permissions.TimeOffTypeCreate, 15 },
            { Application.Auth.Permissions.TimeOffTypeEdit, 16 },
            { Application.Auth.Permissions.TimeOffTypeDelete, 17 },

            { Application.Auth.Permissions.TimeOffBalanceEdit, 18 },

            { Application.Auth.Permissions.HolidayCreate, 19 },
            { Application.Auth.Permissions.HolidayEdit, 20 },
            { Application.Auth.Permissions.HolidayDelete, 21 },

            { Application.Auth.Permissions.PositionCreate, 22 },
            { Application.Auth.Permissions.PositionEdit, 23 },
            { Application.Auth.Permissions.PositionDelete, 24 },
        }
        .ToFrozenDictionary();

    public int BitArrayLength => 128;
}
