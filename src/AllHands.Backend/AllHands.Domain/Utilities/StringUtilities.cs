namespace AllHands.Domain.Utilities;

public static class StringUtilities
{
    public static string GetNormalizedEmail(string email)
        => email.Trim().ToUpperInvariant();
}
