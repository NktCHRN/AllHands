namespace AllHands.WebApi.Contracts;

public record SearchPaginationParametersRequest(int PerPage = 10, int Page = 1, string? Search = null) : PaginationParametersRequest(PerPage, Page);
