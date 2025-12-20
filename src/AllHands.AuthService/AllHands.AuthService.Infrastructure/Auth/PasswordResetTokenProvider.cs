using System.Security.Cryptography;
using AllHands.AuthService.Domain.Models;
using Microsoft.Extensions.Options;

namespace AllHands.AuthService.Infrastructure.Auth;

public sealed class PasswordResetTokenProvider(
    IOptions<PasswordResetTokenProviderOptions> passwordResetTokenProviderOptions,
    TimeProvider timeProvider) : IPasswordResetTokenProvider
{
    public PasswordResetTokenProviderOptions Options => passwordResetTokenProviderOptions.Value;
    
    private const string Alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int TokenLength = 32;

    private const int WorkFactor = 12;

    public (string Token, PasswordResetToken Entity) Generate(Guid globalUserId)
    {
        var token = RandomNumberGenerator.GetString(Alphanumeric, TokenLength); 
        var currentDateTime = timeProvider.GetUtcNow();

        var entity = new PasswordResetToken()
        {
            Id = Guid.CreateVersion7(),
            IssuedAt = currentDateTime,
            ExpiresAt = currentDateTime.Add(Options.LifeTime),
            TokenHash = Hash(token),
            GlobalUserId = globalUserId,
        };
        
        return (token, entity);
    }

    private string Hash(string token)
    {
        return BCrypt.Net.BCrypt.HashPassword(token, WorkFactor, true);
    }

    public bool Verify(string token, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(token, hash, true);
    }
}
