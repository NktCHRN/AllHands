using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Models;
using AllHands.Domain.Utilities;
using AllHands.Infrastructure.Auth;
using AllHands.Infrastructure.Auth.Entities;
using Marten;
using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Data;

public sealed class DevelopmentSeeder(IDocumentStore documentStore, AuthDbContext dbContext, UserManager<AllHandsIdentityUser> userManager, RoleManager<AllHandsRole> roleManager, IPermissionsContainer permissionsContainer)
{
    private readonly Guid _companyId = Guid.Parse("48a9758c-07c3-493d-83d8-d0bf55835112");
    private readonly Guid _adminRoleId = Guid.Parse("f34632fc-b45e-4fcb-9c68-8ba404037a9b");
    private static readonly Guid _adminUserId = Guid.Parse("a502c3da-e280-4193-9ed0-7937620ccd93");
    private readonly Guid _adminGlobalUserId = Guid.Parse("037c90c3-1404-42b8-8121-faa3d86080be");
    private readonly Guid _managerId = Guid.Parse("36a02307-b81d-4b9a-aac3-72485cfb1d08");
    private readonly Guid _vacationId = Guid.Parse("9bee803f-e8b0-4afb-b440-a9578d002adc");
    private readonly Guid _sickLeaveId = Guid.Parse("4bcb9bfe-62fd-4f09-a234-154134699099");
    private readonly Position[] _positions =
    [
        new Position()
        {
            Id = Guid.Parse("be4bcdc3-0530-4e72-9bea-2413d9d44f0f"),
            CompanyId = Guid.Parse("48a9758c-07c3-493d-83d8-d0bf55835112"),
            Name = "Employee",
            NormalizedName = StringUtilities.GetNormalizedName("Employee"),
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _adminUserId
        }
    ];
    
    private IDocumentSession _documentSession;
    
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await using (var documentSession = documentStore.LightweightSession(_companyId.ToString()))
        {
            _documentSession = documentSession;

            await SeedCompany(cancellationToken);
            await SeedTimeOffTypeAsync(cancellationToken);
            await SeedPositions(cancellationToken);
            await SeedRoles(cancellationToken);
            await SeedActiveUser(cancellationToken);
            await SeedInvitedUser(cancellationToken);
        }


        await using (var documentSession2 = documentStore.LightweightSession("9a8953a9-dbd2-4f6a-9151-1367c777b68c"))
        {
            _documentSession = documentSession2;
            
            await CreateActiveUser2(_adminGlobalUserId, cancellationToken);
        }
    }

    private async Task SeedCompany(CancellationToken cancellationToken)
    {
        _documentSession.Insert(new Company()
        {
            Id = _companyId,
            CreatedAt = DateTime.UtcNow,
            EmailDomain = "allhands.com",
            IanaTimeZone = "Europe/Kyiv",
            Name = "AllHands test company",
            Description = "Phasellus id congue massa. Cras massa tortor, gravida ut sollicitudin non, rutrum eget risus. Vivamus lobortis urna et feugiat venenatis. Fusce imperdiet augue id metus scelerisque egestas id a enim. Mauris felis mi, blandit nec molestie ut, sollicitudin at massa. Quisque at laoreet nulla. Suspendisse volutpat justo bibendum mattis mollis. Integer vel ligula posuere velit auctor porta. Proin ut malesuada augue, sed venenatis ex. Sed a dolor consequat, elementum orci eget, tempor dui. Proin ut nisl sed metus finibus dignissim et in tellus. Proin porttitor vitae enim ac lobortis. Fusce et turpis diam. Praesent in sagittis odio. Phasellus egestas cursus nisi quis feugiat. Morbi tempor dapibus metus vel iaculis. ",
            WorkDays = new HashSet<DayOfWeek>()
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
            }
        });
        await _documentSession.SaveChangesAsync(cancellationToken);
        
        _documentSession.Insert(new Company()
        {
            Id = Guid.Parse("9a8953a9-dbd2-4f6a-9151-1367c777b68c"),
            CreatedAt = DateTime.UtcNow,
            EmailDomain = "test.com",
            IanaTimeZone = "Europe/Kyiv",
            Name = "AllHands test company 2",
            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vehicula dignissim dignissim. Ut a erat molestie, tincidunt nisi id, pellentesque erat. Proin pulvinar vel tortor auctor cursus. Vestibulum rutrum lacus at quam elementum laoreet et sit amet leo. Proin congue vulputate ante vel dignissim. Ut a enim luctus, finibus dolor sed, porttitor velit. Sed et diam vel nunc laoreet pharetra. Aliquam aliquam posuere leo, nec tincidunt justo porta vitae. Proin volutpat dui eget urna tempus dictum vitae et lorem. Nunc id fringilla tortor. Etiam ac magna lacinia nunc finibus efficitur vel non nulla. Nam venenatis at lorem nec consequat. ",
            WorkDays = new HashSet<DayOfWeek>()
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday
            }
        });
        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedTimeOffTypeAsync(CancellationToken cancellationToken)
    {
        var timeOffTypes = new TimeOffType[]
        {
            new TimeOffType()
            {
                Id = _vacationId,
                CompanyId = _companyId,
                CreatedAt = DateTime.UtcNow,
                Emoji = "🌴",
                Name = "Vacation",
                DaysPerYear = 20,
                CreatedByUserId = _adminUserId,
                Order = 0
            },
            new TimeOffType()
            {
                Id = _sickLeaveId,
                CompanyId = _companyId,
                CreatedAt = DateTime.UtcNow,
                Emoji = "🤒",
                Name = "Sick leave (Undocumented)",
                DaysPerYear = 10,
                CreatedByUserId = _adminUserId,
                Order = 1
            },
            new TimeOffType()
            {
                Id = Guid.CreateVersion7(),
                CompanyId = _companyId,
                CreatedAt = DateTime.UtcNow,
                Emoji = "🏥",
                Name = "Sick leave (documented)",
                DaysPerYear = 0,
                CreatedByUserId = _adminUserId,
                Order = 3
            }
        };
        
        _documentSession.Insert(timeOffTypes);
        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPositions(CancellationToken cancellationToken)
    {
        _documentSession.Insert(_positions);
        await _documentSession.SaveChangesAsync(cancellationToken);
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
                Claims = claims,
                CreatedAt = DateTime.UtcNow,
            },
            new AllHandsRole()
            {
                Id = Guid.CreateVersion7(),
                CompanyId =  _companyId,
                Name = "Employee",
                IsDefault = true,
                CreatedAt = DateTime.UtcNow,
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

        var stream = _documentSession.Events.StartStream<Employee>(employeeId, new EmployeeCreatedEvent(
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
            new DateOnly(2025, 09, 26)),
            new EmployeeRegisteredEvent(employeeId, employeeId));
        await _documentSession.SaveChangesAsync(cancellationToken);

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
                Id = _adminGlobalUserId,
                DefaultCompanyId =  _companyId,
                Email = "user@example.com",
                NormalizedEmail = "user@example.com".ToUpperInvariant()
            },
            IsInvitationAccepted = true
        };
        _ = await userManager.CreateAsync(identityUser, "P@ssw0rd");

        var vacationBalanceId = Domain.Models.TimeOffBalance.CreateId(employeeId, _vacationId);
        _documentSession.Events.StartStream<TimeOffBalance>(vacationBalanceId, 
            new TimeOffBalanceCreatedEvent(vacationBalanceId, employeeId, _vacationId, 20));
        var sickLeaveBalanceId = Domain.Models.TimeOffBalance.CreateId(employeeId, _sickLeaveId);
        _documentSession.Events.StartStream<TimeOffBalance>(sickLeaveBalanceId, 
            new TimeOffBalanceCreatedEvent(sickLeaveBalanceId, employeeId, _sickLeaveId, 0));
    }

    private async Task CreateActiveUser2(Guid globalUserId, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse("d2c6dfca-0f64-4590-9faf-1b2c5f349236");
        var employeeId = Guid.Parse("686cc75f-35fc-4477-aaa6-e1694cacb327");
        var companyId = Guid.Parse("9a8953a9-dbd2-4f6a-9151-1367c777b68c");

        var stream = _documentSession.Events.StartStream<Employee>(employeeId, new EmployeeCreatedEvent(
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
        await _documentSession.SaveChangesAsync(cancellationToken);

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

        var stream = _documentSession.Events.StartStream<Employee>(employeeId, new EmployeeCreatedEvent(
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
        await _documentSession.SaveChangesAsync(cancellationToken);

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
