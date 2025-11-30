namespace AllHands.Application;

public static class Permissions
{
    public const string RolesCreate = "roles.create";
    public const string RolesEdit = "roles.edit";
    public const string RolesDelete = "roles.delete";

    public const string CompanyEdit = "company.edit";

    public const string EmployeeCreate = "employee.create";
    public const string EmployeeEdit = "employee.edit";
    public const string EmployeeDelete = "employee.delete";
    public const string EmployeeRehire = "employee.rehire";

    public const string TimeOffRequestAdminApprove = "timeoffrequest.adminapprove";     // Can both approve and reject.

    public const string CompanyStatisticsView = "company.statistics.view";
    public const string RolesView = "roles.view";

    public const string NewsCreate = "news.create";
    public const string NewsEdit = "news.edit";
    public const string NewsDelete = "news.delete";

    public const string TimeOffTypeCreate = "timeofftype.create";
    public const string TimeOffTypeEdit = "timeofftype.edit";
    public const string TimeOffTypeDelete = "timeofftype.delete";

    public const string TimeOffBalanceEdit = "timeoffbalance.edit";

    public const string HolidayCreate = "holiday.create";
    public const string HolidayEdit = "holiday.edit";
    public const string HolidayDelete = "holiday.delete";

    public const string PositionCreate = "position.create";
    public const string PositionEdit = "position.edit";
    public const string PositionDelete = "position.delete";
}
