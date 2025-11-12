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
            .HasIndex(x => x.IssuesAt);
    }
}
