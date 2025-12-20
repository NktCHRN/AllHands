using Microsoft.AspNetCore.Identity;

namespace AllHands.AuthService.Infrastructure.Auth;

public static class IdentityUtilities
{
    public static string IdentityErrorsToString(IEnumerable<IdentityError> errors)
    {
        return string.Join(Environment.NewLine, errors.Select(f => f.Description));
    }
}
