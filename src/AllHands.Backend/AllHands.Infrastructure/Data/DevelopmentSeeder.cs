using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Models;
using AllHands.Domain.Utilities;
using AllHands.Infrastructure.Auth;
using AllHands.Infrastructure.Auth.Entities;
using Marten;
using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Data;

public sealed class DevelopmentSeeder(IDocumentSession documentSession, AuthDbContext dbContext, UserManager<AllHandsIdentityUser> userManager, RoleManager<AllHandsRole> roleManager, IPermissionsContainer permissionsContainer)
{
    private readonly Guid _companyId = Guid.Parse("48a9758c-07c3-493d-83d8-d0bf55835112");
    private readonly Guid _adminRoleId = Guid.Parse("f34632fc-b45e-4fcb-9c68-8ba404037a9b");
    private readonly Guid _adminUserId = Guid.Parse("a502c3da-e280-4193-9ed0-7937620ccd93");
    private readonly Guid _managerId = Guid.Parse("36a02307-b81d-4b9a-aac3-72485cfb1d08");
    private readonly Position[] _positions =
    [
        new Position()
        {
            Id = Guid.Parse("be4bcdc3-0530-4e72-9bea-2413d9d44f0f"),
            CompanyId = Guid.Parse("48a9758c-07c3-493d-83d8-d0bf55835112"),
            Name = "Employee"
        }
    ];
    
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await SeedCompany(cancellationToken);
        await SeedPositions(cancellationToken);
        await SeedRoles(cancellationToken);
        await SeedActiveUser(cancellationToken);
        await SeedInvitedUser(cancellationToken);
    }

    private async Task SeedCompany(CancellationToken cancellationToken)
    {
        documentSession.Insert(new Company()
        {
            Id = _companyId,
            CreatedAt = DateTime.UtcNow,
            EmailDomain = "@allhands.com",
            IanaTimeZone = "Europe/Kyiv",
            Name = "AllHands test company"
        });
        await documentSession.SaveChangesAsync(cancellationToken);
        
        documentSession.Insert(new Company()
        {
            Id = Guid.Parse("9a8953a9-dbd2-4f6a-9151-1367c777b68c"),
            CreatedAt = DateTime.UtcNow,
            EmailDomain = "@test.com",
            IanaTimeZone = "Europe/Kyiv",
            Name = "AllHands test company 2"
        });
        await documentSession.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPositions(CancellationToken cancellationToken)
    {
        documentSession.Insert(_positions);
        await documentSession.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedRoles(CancellationToken cancellationToken)
    {
        var claims = new List<AllHandsRoleClaim>();
        foreach (var permission in permissionsContainer.Permissions)
        {
            claims.Add(new AllHandsRoleClaim()
            {
                Id = Guid.CreateVersion7(),
                ClaimType = AuthConstants.PermissionClaimName,
                ClaimValue = permission.Key
            });
        }
        var roles = new AllHandsRole[]
        {
            new AllHandsRole()
            {
                Id = _adminRoleId,
                CompanyId =  _companyId,
                Name = "Admin",
                Claims = claims
            }
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }
    }

    private async Task SeedActiveUser(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse("a502c3da-e280-4193-9ed0-7937620ccd93");
        var employeeId = Guid.Parse("36a02307-b81d-4b9a-aac3-72485cfb1d08");

        var stream = documentSession.Events.StartStream<Employee>(employeeId, new EmployeeCreatedEvent(
            employeeId,
            _adminUserId,
            userId,
            _companyId,
            _positions[0].Id,
            _managerId,
            "user@example.com",
            StringUtilities.GetNormalizedEmail("user@example.com"),
            "Anastasiia",
            "Vadymivna",
            "Linchuk",
            "+380681112233",
            new DateOnly(2025, 11, 26)),
            new EmployeeRegisteredEvent(employeeId, employeeId));
        await documentSession.SaveChangesAsync(cancellationToken);

        var globalUserId = Guid.Parse("037c90c3-1404-42b8-8121-faa3d86080be");

        var identityUser = new AllHandsIdentityUser()
        {
            Id = userId,
            Email = "user@example.com",
            UserName = $"user@example.com_{_companyId}",
            FirstName = "Anastasiia",
            MiddleName = "Vadymivna",
            LastName = "Linchuk",
            PhoneNumber = "+380681112233",
            CompanyId = _companyId,
            Roles = [new AllHandsUserRole()
            {
                RoleId = _adminRoleId,
            }],
            GlobalUser = new AllHandsGlobalUser()
            {
                Id = globalUserId,
                DefaultCompanyId =  _companyId,
                Email = "user@example.com",
                NormalizedEmail = "user@example.com".ToUpperInvariant()
            },
            IsInvitationAccepted = true
        };
        _ = await userManager.CreateAsync(identityUser, "P@ssw0rd");

        await CreateActiveUser2(globalUserId, cancellationToken);
    }

    private async Task CreateActiveUser2(Guid globalUserId, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse("d2c6dfca-0f64-4590-9faf-1b2c5f349236");
        var employeeId = Guid.Parse("686cc75f-35fc-4477-aaa6-e1694cacb327");
        var companyId = Guid.Parse("9a8953a9-dbd2-4f6a-9151-1367c777b68c");

        var stream = documentSession.Events.StartStream<Employee>(employeeId, new EmployeeCreatedEvent(
                employeeId,
                _adminUserId,
                userId,
                companyId,
                _positions[0].Id,
                _managerId,
                "user@example.com",
                StringUtilities.GetNormalizedEmail("user@example.com"),
                "Anastasiia",
                "Vadymivna",
                "Linchuk",
                "+380681112233",
                new DateOnly(2025, 11, 26)),
            new EmployeeRegisteredEvent(employeeId, employeeId));
        await documentSession.SaveChangesAsync(cancellationToken);

        var identityUser = new AllHandsIdentityUser()
        {
            Id = userId,
            Email = "user@example.com",
            UserName = $"user@example.com_{companyId}",
            FirstName = "Anastasiia",
            MiddleName = "Vadymivna",
            LastName = "Linchuk",
            PhoneNumber = "+380681112233",
            CompanyId = companyId,
            GlobalUserId = globalUserId,
            IsInvitationAccepted = true
        };
        _ = await userManager.CreateAsync(identityUser, "P@ssw0rd");
    }
    
    private async Task SeedInvitedUser(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse("69d1e588-1d16-4647-8f49-359c0a76f9a7");
        var employeeId = Guid.Parse("b8900ae9-011b-4184-b813-7d35e83f8082");

        var stream = documentSession.Events.StartStream<Employee>(employeeId, new EmployeeCreatedEvent(
                employeeId,
                _adminUserId,
                userId,
                _companyId,
                _positions[0].Id,
                _managerId,
                "14nik20@gmail.com",
                StringUtilities.GetNormalizedEmail("14nik20@gmail.com"),
                "Nikita",
                "Mykolaiovych",
                "Chernikov",
                "+380682223344",
                new DateOnly(2025, 11, 27)));
        await documentSession.SaveChangesAsync(cancellationToken);

        var identityUser = new AllHandsIdentityUser()
        {
            Id = userId,
            Email = "14nik20@gmail.com",
            UserName = $"14nik20@gmail.com_{_companyId}",
            FirstName = "Nikita",
            MiddleName = "Mykolaiovych",
            LastName = "Chernikov",
            PhoneNumber = "+380682223344",
            CompanyId = _companyId,
            GlobalUser = new AllHandsGlobalUser()
            {
                Id = Guid.NewGuid(),
                DefaultCompanyId =  _companyId,
                Email = "14nik20@gmail.com",
                NormalizedEmail = "14nik20@gmail.com".ToUpperInvariant()
            }
        };
        _ = await userManager.CreateAsync(identityUser);

        var invitation = new Invitation()
        {
            Id = Guid.Parse("3f8ecd64-897c-4224-90f6-1754ea4d631f"),
            TokenHash = BCrypt.Net.BCrypt.HashPassword("1122334455", 12, true),
            IssuerId = Guid.Parse("a502c3da-e280-4193-9ed0-7937620ccd93"),
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            UserId = userId,
        };
        await dbContext.Invitations.AddAsync(invitation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
