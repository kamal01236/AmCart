using CatalogService.Api.Domain;

namespace CatalogService.Api.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Product>> SearchAsync(string? query, Guid? categoryId, int page, int pageSize, CancellationToken cancellationToken);
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken);
}
