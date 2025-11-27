using AllHands.Infrastructure.Auth.Entities;

namespace AllHands.Infrastructure.Auth;

public interface IPasswordResetTokenProvider
{
    (string Token, PasswordResetToken Entity) Generate(Guid globalUserId);
    PasswordResetTokenProviderOptions Options { get; }
    bool Verify(string token, string hash);
}