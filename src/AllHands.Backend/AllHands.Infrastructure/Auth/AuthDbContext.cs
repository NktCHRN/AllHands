using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AllHands.Infrastructure.Auth;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options)
    : IdentityDbContext<AllHandsIdentityUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<AllHandsSession> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AllHandsSession>()
            .Property(s => s.TicketValue)
            .HasColumnType("jsonb");
        modelBuilder.Entity<AllHandsSession>()
            .HasKey(x => x.Key);
        modelBuilder.Entity<AllHandsSession>()
            .HasIndex(x => x.IssuedAt);
        
        modelBuilder.Entity<Invitation>()
            .Property(x => x.TokenHash)
            .HasMaxLength(64);
        modelBuilder.Entity<Invitation>()
            .HasIndex(x => new { x.ExpiresAt, x.TokenHash });
        modelBuilder.Entity<Invitation>()
            .HasOne(x => x.Issuer)
            .WithMany(x => x.IssuedInvitations)
            .HasForeignKey(x => x.IssuerId);
        modelBuilder.Entity<Invitation>()
            .HasOne(x => x.User)
            .WithMany(x => x.Invitations)
            .HasForeignKey(x => x.UserId);
    }
}
