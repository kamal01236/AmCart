using CatalogService.Api.Domain;
using CatalogService.Api.Transport;

namespace CatalogService.Api.Services;

public static class MappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.Currency,
            product.CategoryId,
            product.Category?.Name ?? string.Empty,
            product.IsActive,
            product.Tags.ToArray(),
            product.CreatedAtUtc,
            product.UpdatedAtUtc,
            product.PublishedAtUtc);
    }

    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.CreatedAtUtc,
            category.UpdatedAtUtc);
    }
}
