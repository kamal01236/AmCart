namespace SearchService.Api.Transport;

public record ProductSearchResultItem(
    Guid Id,
    string Sku,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    Guid CategoryId,
    string CategoryName,
    IReadOnlyCollection<string> Tags,
    double? Score,
    double? Rating,
    int? ReviewCount,
    int? Popularity,
    string? ImageUrl,
    DateTime? PublishedAtUtc);
