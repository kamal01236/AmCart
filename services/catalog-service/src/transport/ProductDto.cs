namespace CatalogService.Api.Transport;

public record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    Guid CategoryId,
    string CategoryName,
    bool IsActive,
    IReadOnlyCollection<string> Tags,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    DateTime? PublishedAtUtc);
