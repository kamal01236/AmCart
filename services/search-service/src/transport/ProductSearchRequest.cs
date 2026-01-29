namespace SearchService.Api.Transport;

public record ProductSearchRequest(
    string? Query,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    IReadOnlyCollection<string>? Tags,
    string? Sort,
    int Page = 0,
    int PageSize = 20);
