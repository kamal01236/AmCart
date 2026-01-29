namespace CatalogService.Api.Transport;

public record UpsertProductRequest(
    string Sku,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    Guid CategoryId,
    bool IsActive,
    IReadOnlyCollection<string> Tags);
