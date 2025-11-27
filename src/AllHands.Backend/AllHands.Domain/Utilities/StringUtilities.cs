namespace AllHands.Domain.Utilities;

public static class StringUtilities
{
    public static string GetNormalizedEmail(string email)
        => email.Trim().ToUpperInvariant();
    
    public static string GetNormalizedName(string name)
        => name.Trim().ToUpperInvariant();
}
