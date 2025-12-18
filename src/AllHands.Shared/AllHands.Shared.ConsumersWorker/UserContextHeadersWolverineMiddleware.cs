using System.Collections;
using System.Text.Json;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Infrastructure.UserContext;
using Wolverine;

namespace AllHands.Shared.ConsumersWorker;

public sealed class UserContextHeadersWolverineMiddleware(IUserContextAccessor userContextAccessor)
{
    public void Before(Envelope envelope)
    {
        var context = userContextAccessor.UserContext;
        if (context is null)
        {
            return;
        }
        
        if (!string.IsNullOrEmpty(envelope.Headers[UserContextHeaders.Id]))
        {
            context.Id = Guid.Parse(envelope.Headers[UserContextHeaders.Id]!);
        }

        if (!string.IsNullOrEmpty(envelope.Headers[UserContextHeaders.Email]))
        {
            context.Email = envelope.Headers[UserContextHeaders.Email]!;
        }
        
        context.PhoneNumber = envelope.Headers[UserContextHeaders.PhoneNumber]!;
        
        if (!string.IsNullOrEmpty(envelope.Headers[UserContextHeaders.CompanyId]))
        {
            context.CompanyId = Guid.Parse(envelope.Headers[UserContextHeaders.CompanyId]!);
        }
        
        if (!string.IsNullOrEmpty(envelope.Headers[UserContextHeaders.EmployeeId]))
        {
            context.EmployeeId = Guid.Parse(envelope.Headers[UserContextHeaders.EmployeeId]!);
        }

        if (!string.IsNullOrEmpty(envelope.Headers[UserContextHeaders.FirstName]))
        {
            context.FirstName = envelope.Headers[UserContextHeaders.FirstName]!;
        }

        context.MiddleName = envelope.Headers[UserContextHeaders.MiddleName]!;
        
        if (!string.IsNullOrEmpty(envelope.Headers[UserContextHeaders.LastName]))
        {
            context.LastName = envelope.Headers[UserContextHeaders.LastName]!;
        }

        if (!string.IsNullOrEmpty(envelope.Headers[UserContextHeaders.Roles]))
        {
            context.Roles = JsonSerializer.Deserialize<List<string>>(envelope.Headers[UserContextHeaders.Roles]!) ?? [];
        }

        if (!string.IsNullOrEmpty(envelope.Headers[UserContextHeaders.Permissions]))
        {
            context.Permissions =
                new BitArray(Convert.FromBase64String(envelope.Headers[UserContextHeaders.Permissions]!));
        }
    }
}
