namespace CatalogService.Api.Transport;

public record UpsertCategoryRequest(
    string Name,
    string Slug,
    string Description);
