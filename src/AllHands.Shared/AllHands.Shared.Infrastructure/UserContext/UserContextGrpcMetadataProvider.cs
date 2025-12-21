using AllHands.Shared.Infrastructure.Utilities;

namespace AllHands.Shared.Infrastructure.UserContext;

public static class UserContextGrpcMetadataProvider
{
    public static Grpc.Core.Metadata GetMetadata(Domain.UserContext.IUserContext userContext)
    {
        var metadata = new Grpc.Core.Metadata()
        {
            {UserContextHeaders.Id, userContext.Id.ToString()},
            {UserContextHeaders.CompanyId, userContext.CompanyId.ToString()},
            {UserContextHeaders.EmployeeId, userContext.EmployeeId.ToString()},
            {UserContextHeaders.Email, userContext.Email},
            {UserContextHeaders.FirstName, userContext.FirstName},
            {UserContextHeaders.LastName, userContext.LastName},
            {UserContextHeaders.PermissionsBin, userContext.Permissions.ToByteArray()}
        };

        if (!string.IsNullOrEmpty(userContext.PhoneNumber))
        {
            metadata.Add(UserContextHeaders.PhoneNumber, userContext.PhoneNumber);
        }

        if (!string.IsNullOrEmpty(userContext.MiddleName))
        {
            metadata.Add(UserContextHeaders.MiddleName, userContext.MiddleName);
        }

        foreach (var role in userContext.Roles)
        {
            metadata.Add(UserContextHeaders.Roles, role);
        }
        
        return metadata;
    }
}
