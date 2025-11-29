using AllHands.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AllHands.Infrastructure.Auth;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options)
    : IdentityDbContext<AllHandsIdentityUser, AllHandsRole, Guid, IdentityUserClaim<Guid>, AllHandsUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{
    public DbSet<AllHandsSession> Sessions { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<AllHandsRoleClaim> AllHandsRoleClaims { get; set; }
    public DbSet<AllHandsGlobalUser> GlobalUsers { get; set; }
    public DbSet<RecalculateCompanySessionsTask> RecalculateCompanySessionsTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AllHandsSession>()
            .Property(s => s.TicketValue)
            .HasColumnType("bytea");
        modelBuilder.Entity<AllHandsSession>()
            .HasKey(x => x.Key);
        modelBuilder.Entity<AllHandsSession>()
            .HasIndex(x => x.IssuedAt);
        modelBuilder.Entity<AllHandsSession>()
            .HasIndex(x => new { x.UserId, x.ExpiresAt });
        
        modelBuilder.Entity<Invitation>()
            .Property(x => x.TokenHash)
            .HasMaxLength(64);
        modelBuilder.Entity<Invitation>()
            .HasOne(x => x.Issuer)
            .WithMany(x => x.IssuedInvitations)
            .HasForeignKey(x => x.IssuerId);
        modelBuilder.Entity<Invitation>()
            .HasOne(x => x.User)
            .WithMany(x => x.Invitations)
            .HasForeignKey(x => x.UserId);
        modelBuilder.Entity<Invitation>()
            .HasIndex(x => x.IssuedAt)
            .IsDescending(true);
        
        modelBuilder.Entity<PasswordResetToken>()
            .Property(x => x.TokenHash)
            .HasMaxLength(64);
        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(x => x.GlobalUser)
            .WithMany(x => x.PasswordResetTokens)
            .HasForeignKey(x => x.GlobalUserId);
        modelBuilder.Entity<PasswordResetToken>()
            .HasIndex(x => x.IssuedAt)
            .IsDescending(true);
        
        modelBuilder.Entity<AllHandsRole>()
            .Property(x => x.CompanyId)
            .IsRequired();
        var index = modelBuilder.Entity<AllHandsRole>()
            .HasIndex(u => new { u.NormalizedName }).Metadata;
        _ = modelBuilder.Entity<AllHandsRole>().Metadata.RemoveIndex(index.Properties);
        modelBuilder.Entity<AllHandsRole>()
            .HasIndex(e => new {e.CompanyId, e.NormalizedName})
            .HasDatabaseName("RoleNameIndex")
            .IsUnique()
            .HasFilter($"\"{nameof(AllHandsRole.DeletedAt)}\" IS NULL");
        modelBuilder.Entity<AllHandsRole>()
            .HasIndex(e => e.CompanyId)
            .IsUnique()
            .HasFilter($"\"{nameof(AllHandsRole.DeletedAt)}\" IS NULL AND \"{nameof(AllHandsRole.IsDefault)}\" = true");
        
        modelBuilder.Entity<AllHandsRole>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<AllHandsRole>()
            .HasMany(x => x.Claims)
            .WithOne()
            .HasForeignKey(x => x.RoleId)
            .HasPrincipalKey(x => x.Id);
        modelBuilder.Entity<AllHandsRole>()
            .HasQueryFilter(x => !x.DeletedAt.HasValue);
        modelBuilder.Entity<AllHandsRole>()
            .HasMany(x => x.Users)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .HasPrincipalKey(x => x.Id);
        
        modelBuilder.Entity<AllHandsIdentityUser>()
            .Property(x => x.FirstName)
            .HasMaxLength(255);
        modelBuilder.Entity<AllHandsIdentityUser>()
            .Property(x => x.MiddleName)
            .HasMaxLength(255);
        modelBuilder.Entity<AllHandsIdentityUser>()
            .Property(x => x.LastName)
            .HasMaxLength(255);
        modelBuilder.Entity<AllHandsIdentityUser>()
            .HasMany(x => x.Roles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id);
        modelBuilder.Entity<AllHandsIdentityUser>()
            .HasIndex(x => x.NormalizedUserName)
            .IsUnique()
            .HasFilter($"\"{nameof(AllHandsRole.DeletedAt)}\" IS NULL");
        modelBuilder.Entity<AllHandsIdentityUser>()
            .HasOne(x => x.GlobalUser)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.GlobalUserId);
        modelBuilder.Entity<AllHandsIdentityUser>()
            .Navigation(u => u.GlobalUser)
            .AutoInclude();
        modelBuilder.Entity<AllHandsIdentityUser>()
            .HasQueryFilter(x => !x.DeletedAt.HasValue);
        
        modelBuilder.Entity<AllHandsRoleClaim>()
            .Property(x => x.ClaimType)
            .HasMaxLength(255);
        modelBuilder.Entity<AllHandsRoleClaim>()
            .Property(x => x.ClaimValue)
            .HasMaxLength(255);
        
        modelBuilder.Entity<AllHandsGlobalUser>()
            .Property(x => x.Email)
            .HasMaxLength(256);
        modelBuilder.Entity<AllHandsGlobalUser>()
            .Property(x => x.NormalizedEmail)
            .HasMaxLength(256);

        modelBuilder.Entity<RecalculateCompanySessionsTask>()
            .HasOne<AllHandsIdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.RequesterUserId);
        modelBuilder.Entity<RecalculateCompanySessionsTask>()
            .HasIndex(x => x.RequestedAt)
            .HasFilter($"\"{nameof(RecalculateCompanySessionsTask.CompletedAt)}\" IS NULL");
    }
}
