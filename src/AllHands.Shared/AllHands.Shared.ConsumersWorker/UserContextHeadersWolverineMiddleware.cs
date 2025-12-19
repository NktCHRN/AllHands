using System.Collections;
using System.Text.Json;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Infrastructure.UserContext;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace AllHands.Shared.ConsumersWorker;

public sealed class UserContextHeadersWolverineMiddleware(IUserContextSetuper userContextSetuper, ILogger<UserContextHeadersWolverineMiddleware> logger)
{
    public void Before(Envelope envelope)
    {
        var context = new UserContext();
        
        string? GetHeader(string key)
            => envelope.Headers.TryGetValue(key, out var s) && !string.IsNullOrWhiteSpace(s) ? s : null;

        // Id
        var id = GetHeader(UserContextHeaders.Id);
        if (id is not null && Guid.TryParse(id, out var idGuid))
            context.Id = idGuid;

        // Email
        var email = GetHeader(UserContextHeaders.Email);
        if (email is not null)
            context.Email = email;

        context.PhoneNumber = GetHeader(UserContextHeaders.PhoneNumber);

        // CompanyId
        var companyId = GetHeader(UserContextHeaders.CompanyId);
        if (companyId is not null && Guid.TryParse(companyId, out var companyGuid))
            context.CompanyId = companyGuid;

        // EmployeeId
        var employeeId = GetHeader(UserContextHeaders.EmployeeId);
        if (employeeId is not null && Guid.TryParse(employeeId, out var employeeGuid))
            context.EmployeeId = employeeGuid;

        // FirstName
        var firstName = GetHeader(UserContextHeaders.FirstName);
        if (firstName is not null)
            context.FirstName = firstName;

        context.MiddleName = GetHeader(UserContextHeaders.MiddleName);

        // LastName
        var lastName = GetHeader(UserContextHeaders.LastName);
        if (lastName is not null)
            context.LastName = lastName;

        // Roles (JSON)
        var rolesJson = GetHeader(UserContextHeaders.Roles);
        if (rolesJson is not null)
        {
            try
            {
                context.Roles = JsonSerializer.Deserialize<List<string>>(rolesJson) ?? [];
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to parse roles from JSON");
            }
        }

        // Permissions (Base64 -> BitArray)
        var permissionsB64 = GetHeader(UserContextHeaders.Permissions);
        if (permissionsB64 is not null)
        {
            try
            {
                context.Permissions = new BitArray(Convert.FromBase64String(permissionsB64));
            }
            catch (FormatException ex)
            {
                logger.LogError(ex, "Failed to parse permissions");
            }
        }
        
        userContextSetuper.Push(context);
    }
}
