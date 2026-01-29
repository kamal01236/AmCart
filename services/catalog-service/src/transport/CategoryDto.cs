namespace CatalogService.Api.Transport;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
