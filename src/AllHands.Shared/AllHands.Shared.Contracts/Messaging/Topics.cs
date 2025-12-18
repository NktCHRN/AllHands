namespace AllHands.Shared.Contracts.Messaging;

public static class Topics
{
    public const string NewsPost = $"{Services.NewsService}_news-post-events";
    public const string Employee = $"{Services.EmployeeService}_employee-events";
    public const string Company = $"{Services.EmployeeService}_company-events";
}
