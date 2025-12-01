using AllHands.Application;
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
    private static readonly Guid CompanyId = Guid.Parse("48a9758c-07c3-493d-83d8-d0bf55835112");
    private static readonly Guid AdminRoleId = Guid.Parse("f34632fc-b45e-4fcb-9c68-8ba404037a9b");
    private static readonly Guid AdminUserId = Guid.Parse("a502c3da-e280-4193-9ed0-7937620ccd93");
    private static readonly Guid AdminGlobalUserId = Guid.Parse("037c90c3-1404-42b8-8121-faa3d86080be");
    private static readonly Guid ManagerId = Guid.Parse("36a02307-b81d-4b9a-aac3-72485cfb1d08");
    private static readonly Guid VacationId = Guid.Parse("9bee803f-e8b0-4afb-b440-a9578d002adc");
    private const string AdminEmail = "14nik20cloud@gmail.com";

    private readonly Position _adminUserPosition =
        new Position()
        {
            Id = Guid.CreateVersion7(),
            CompanyId = CompanyId,
            Name = "System Administrator",
            NormalizedName = StringUtilities.GetNormalizedName("System Administrator"),
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = AdminUserId
        };
    
    private IDocumentSession _documentSession = null!;
    
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await using (var documentSession = documentStore.LightweightSession(CompanyId.ToString()))
        {
            _documentSession = documentSession;

            await SeedCompany(cancellationToken);
            await SeedTimeOffTypeAsync(cancellationToken);
            await SeedHolidaysAsync(cancellationToken);
            await SeedPositions(cancellationToken);
            await SeedRoles(cancellationToken);
            await SeedActiveUser(cancellationToken);
        }
    }

    private async Task SeedCompany(CancellationToken cancellationToken)
    {
        _documentSession.Insert(new Company()
        {
            Id = CompanyId,
            CreatedAt = DateTime.UtcNow,
            EmailDomain = "allhands.com",
            IanaTimeZone = "Europe/Kyiv",
            Name = "AllHands Test Company",
            Description = "Phasellus id congue massa. Cras massa tortor, gravida ut sollicitudin non, rutrum eget risus. Vivamus lobortis urna et feugiat venenatis. Fusce imperdiet augue id metus scelerisque egestas id a enim. Mauris felis mi, blandit nec molestie ut, sollicitudin at massa. Quisque at laoreet nulla. Suspendisse volutpat justo bibendum mattis mollis. Integer vel ligula posuere velit auctor porta. Proin ut malesuada augue, sed venenatis ex. Sed a dolor consequat, elementum orci eget, tempor dui. Proin ut nisl sed metus finibus dignissim et in tellus. Proin porttitor vitae enim ac lobortis. Fusce et turpis diam. Praesent in sagittis odio. Phasellus egestas cursus nisi quis feugiat. Morbi tempor dapibus metus vel iaculis. ",
            WorkDays = new HashSet<DayOfWeek>()
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
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
                Id = VacationId,
                CompanyId = CompanyId,
                CreatedAt = DateTime.UtcNow,
                Emoji = "🌴",
                Name = "Vacation",
                DaysPerYear = 20,
                CreatedByUserId = AdminUserId,
                Order = 1
            },
            new TimeOffType()
            {
                Id = Guid.CreateVersion7(),
                CompanyId = CompanyId,
                CreatedAt = DateTime.UtcNow,
                Emoji = "🤒",
                Name = "Sick leave (Undocumented)",
                DaysPerYear = 0,
                CreatedByUserId = AdminUserId,
                Order = 2
            },
            new TimeOffType()
            {
                Id = Guid.CreateVersion7(),
                CompanyId = CompanyId,
                CreatedAt = DateTime.UtcNow,
                Emoji = "🏥",
                Name = "Sick leave (documented)",
                DaysPerYear = 0,
                CreatedByUserId = AdminUserId,
                Order = 3
            }
        };
        
        _documentSession.Insert(timeOffTypes);
        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedHolidaysAsync(CancellationToken cancellationToken)
    {
        var holidays = new Holiday[]
        {
            new Holiday()
            {
                Id = Guid.CreateVersion7(),
                CompanyId = CompanyId,
                Date = new DateOnly(2025, 12, 25),
                Name = "Christmas",
                CreatedAt = DateTime.UtcNow,
            },
            new Holiday()
            {
                Id = Guid.CreateVersion7(),
                CompanyId = CompanyId,
                Date = new DateOnly(2026, 01, 01),
                Name = "New Year",
                CreatedAt = DateTime.UtcNow,
            }
        };
        
        _documentSession.Insert(holidays);
        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedPositions(CancellationToken cancellationToken)
    {
        _documentSession.Insert(_adminUserPosition);
        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedRoles(CancellationToken cancellationToken)
    {
        var roles = new AllHandsRole[]
        {
            new AllHandsRole()
            {
                Id = AdminRoleId,
                CompanyId =  CompanyId,
                Name = "Admin",
                Claims = permissionsContainer.Permissions.Select(p => new AllHandsRoleClaim()
                {
                    Id = Guid.CreateVersion7(),
                    ClaimType = AuthConstants.PermissionClaimName,
                    ClaimValue = p.Key
                }).ToList(),
                CreatedAt = DateTime.UtcNow,
            },
            new AllHandsRole()
            {
                Id = Guid.CreateVersion7(),
                CompanyId =  CompanyId,
                Name = "HR",
                Claims = new List<AllHandsRoleClaim>()
                {
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.EmployeeCreate
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.EmployeeCreate
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.NewsCreate
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.NewsEdit
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.NewsDelete
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.RolesView
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.TimeOffBalanceEdit
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.TimeOffRequestAdminApprove
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.HolidayCreate
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.HolidayEdit
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.HolidayDelete
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.PositionCreate
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.PositionEdit
                    },
                    new AllHandsRoleClaim()
                    {
                        Id = Guid.CreateVersion7(),
                        ClaimType = AuthConstants.PermissionClaimName,
                        ClaimValue = Permissions.PositionDelete
                    },
                },
                CreatedAt = DateTime.UtcNow,
            },
            new AllHandsRole()
            {
                Id = Guid.CreateVersion7(),
                CompanyId =  CompanyId,
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
        var stream = _documentSession.Events.StartStream<Employee>(ManagerId, new EmployeeCreatedEvent(
            ManagerId,
            AdminUserId,
            AdminUserId,
            CompanyId,
            _adminUserPosition.Id,
            ManagerId,
            AdminEmail,
            StringUtilities.GetNormalizedEmail(AdminEmail),
            "Nikita",
            "Mykolaiovych",
            "Chernikov",
            "+380681112233",
            new DateOnly(2025, 09, 26)),
            new EmployeeRegisteredEvent(ManagerId, ManagerId));
        await _documentSession.SaveChangesAsync(cancellationToken);

        var identityUser = new AllHandsIdentityUser()
        {
            Id = AdminUserId,
            Email = AdminEmail,
            UserName = $"{AdminEmail}_{CompanyId}",
            FirstName = "Nikita",
            MiddleName = "Mykolaiovych",
            LastName = "Chernikov",
            PhoneNumber = "+380681112233",
            CompanyId = CompanyId,
            Roles = [new AllHandsUserRole()
            {
                RoleId = AdminRoleId,
            }],
            GlobalUser = new AllHandsGlobalUser()
            {
                Id = AdminGlobalUserId,
                DefaultCompanyId =  CompanyId,
                Email = AdminEmail,
                NormalizedEmail = StringUtilities.GetNormalizedEmail(AdminEmail)
            },
            IsInvitationAccepted = true
        };
        _ = await userManager.CreateAsync(identityUser, "P@ssw0rd");

        var vacationBalanceId = Domain.Models.TimeOffBalance.CreateId(ManagerId, VacationId);
        _documentSession.Events.StartStream<TimeOffBalance>(vacationBalanceId, 
            new TimeOffBalanceCreatedEvent(vacationBalanceId, ManagerId, VacationId, 20));
    }

}
