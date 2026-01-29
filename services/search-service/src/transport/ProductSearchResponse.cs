namespace SearchService.Api.Transport;

public record ProductSearchResponse(
    long Total,
    int Page,
    int PageSize,
    IReadOnlyCollection<ProductSearchResultItem> Results);
