namespace AllHands.Domain.Utilities;

public static class StringUtilities
{
    public static string GetNormalizedEmail(string email)
        => email.Trim().ToUpperInvariant();
    
    public static string GetNormalizedName(string name)
        => name.Trim().ToUpperInvariant();
    
    public static string GetFullName(string firstName, string? middleName, string lastName)
        => string.Join(" ", new string?[] {firstName, middleName, lastName}.Where(s => !string.IsNullOrEmpty(s)));
}
