using AllHands.AuthService.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace AllHands.AuthService.Infrastructure.Auth;

public sealed class BCryptUserPasswordHasher : IPasswordHasher<AllHandsIdentityUser>
{
    private const int WorkFactor = 12;
    
    public string HashPassword(AllHandsIdentityUser user, string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor, true);
    }

    public PasswordVerificationResult VerifyHashedPassword(AllHandsIdentityUser user, string hashedPassword,
        string providedPassword)
    {
        var isCorrect = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword, true);
        
        return isCorrect ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}
