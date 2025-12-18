namespace AllHands.Shared.Contracts.Rest;

public record PaginationParametersRequest(int PerPage = 10, int Page = 1);
