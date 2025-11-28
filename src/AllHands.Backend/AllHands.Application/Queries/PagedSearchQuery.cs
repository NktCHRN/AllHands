namespace AllHands.Application.Queries;

public record PagedSearchQuery(int PerPage, int Page, string? Search) : PagedQuery(PerPage, Page);
