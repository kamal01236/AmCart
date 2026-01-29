using CatalogService.Api.Transport;

namespace CatalogService.Api.Services;

public interface IProductService
{
    Task<ProductDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ProductDto>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken);
    Task<ProductDto> CreateAsync(UpsertProductRequest request, CancellationToken cancellationToken);
    Task<ProductDto> UpdateAsync(Guid id, UpsertProductRequest request, CancellationToken cancellationToken);
    Task PublishIndexEventAsync(Guid productId, CancellationToken cancellationToken);
}
