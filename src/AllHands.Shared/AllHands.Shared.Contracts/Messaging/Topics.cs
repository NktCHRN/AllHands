namespace AllHands.Shared.Contracts.Messaging;

public static class Topics
{
    public const string News = $"{Services.NewsService}_news-events";
    public const string Employee = $"{Services.EmployeeService}_employee-events";
    public const string Company = $"{Services.EmployeeService}_company-events";
}
