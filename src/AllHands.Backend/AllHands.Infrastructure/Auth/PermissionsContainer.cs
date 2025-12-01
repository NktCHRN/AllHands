using System.Collections.Frozen;
using AllHands.Application.Abstractions;

namespace AllHands.Infrastructure.Auth;

public sealed class PermissionsContainer : IPermissionsContainer
{
    public IReadOnlyDictionary<string, int> Permissions { get; } = new Dictionary<string, int>()
        {
            { Application.Permissions.RolesCreate, 1 },
            { Application.Permissions.RolesEdit, 2 },
            { Application.Permissions.RolesDelete, 3 },

            { Application.Permissions.CompanyEdit, 4 },

            { Application.Permissions.EmployeeCreate, 5 },
            { Application.Permissions.EmployeeEdit, 6 },
            { Application.Permissions.EmployeeDelete, 7 },

            { Application.Permissions.TimeOffRequestAdminApprove, 8 },

            { Application.Permissions.CompanyStatisticsView, 9 },
            { Application.Permissions.RolesView, 10 },

            { Application.Permissions.NewsCreate, 11 },
            { Application.Permissions.NewsEdit, 12 },
            { Application.Permissions.NewsDelete, 13 },
            
            // 14 is empty for now.

            { Application.Permissions.TimeOffTypeCreate, 15 },
            { Application.Permissions.TimeOffTypeEdit, 16 },
            { Application.Permissions.TimeOffTypeDelete, 17 },

            { Application.Permissions.TimeOffBalanceEdit, 18 },

            { Application.Permissions.HolidayCreate, 19 },
            { Application.Permissions.HolidayEdit, 20 },
            { Application.Permissions.HolidayDelete, 21 },

            { Application.Permissions.PositionCreate, 22 },
            { Application.Permissions.PositionEdit, 23 },
            { Application.Permissions.PositionDelete, 24 },
        }
        .ToFrozenDictionary();

    public int PermissionsLength => 128;
}
