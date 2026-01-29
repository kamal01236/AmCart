namespace CatalogService.Api.Transport;

public record ProductSearchRequest(
    string? Query,
    Guid? CategoryId,
    int Page = 0,
    int PageSize = 20);
