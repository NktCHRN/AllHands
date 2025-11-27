namespace AllHands.Application.Features.Company.Get;

public sealed record GetCompanyResult(
    Guid Id,
    string Name,
    string EmailDomain,
    string IanaTimeZone,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? DeletedAt);
    