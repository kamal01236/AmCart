namespace SearchService.Api.Services;

public record SearchQuery(
    string? Query,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    IReadOnlyCollection<string> Tags,
    SearchSortOption Sort,
    int Page,
    int PageSize);
