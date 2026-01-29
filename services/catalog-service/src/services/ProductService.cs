using CatalogService.Api.Domain;
using CatalogService.Api.Repositories;
using CatalogService.Api.Transport;

namespace CatalogService.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISearchIndexer _searchIndexer;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ISearchIndexer searchIndexer)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _searchIndexer = searchIndexer;
    }

    public async Task<ProductDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product?.ToDto();
    }

    public async Task<IReadOnlyCollection<ProductDto>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken)
    {
        var items = await _productRepository.SearchAsync(request.Query, request.CategoryId, Math.Max(request.Page, 0), Math.Clamp(request.PageSize, 1, 100), cancellationToken);
        return items.Select(p => p.ToDto()).ToArray();
    }

    public async Task<ProductDto> CreateAsync(UpsertProductRequest request, CancellationToken cancellationToken)
    {
        await EnsureCategoryExists(request.CategoryId, cancellationToken);
        if (await _productRepository.GetBySkuAsync(request.Sku, cancellationToken) is not null)
        {
            throw new InvalidOperationException($"SKU {request.Sku} already exists");
        }

        var product = new Product
        {
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Currency = request.Currency.ToUpperInvariant(),
            CategoryId = request.CategoryId,
            IsActive = request.IsActive,
            Tags = request.Tags?.ToArray() ?? Array.Empty<string>(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            PublishedAtUtc = request.IsActive ? DateTime.UtcNow : null
        };

        var saved = await _productRepository.CreateAsync(product, cancellationToken);
        await _searchIndexer.PublishProductChangeAsync(saved.Id, cancellationToken);
        return saved.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpsertProductRequest request, CancellationToken cancellationToken)
    {
        await EnsureCategoryExists(request.CategoryId, cancellationToken);
        var product = await _productRepository.GetByIdAsync(id, cancellationToken) ??
                      throw new KeyNotFoundException("Product not found");

        if (!string.Equals(product.Sku, request.Sku, StringComparison.OrdinalIgnoreCase))
        {
            if (await _productRepository.GetBySkuAsync(request.Sku, cancellationToken) is not null)
            {
                throw new InvalidOperationException($"SKU {request.Sku} already exists");
            }
        }

        product.Sku = request.Sku;
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Currency = request.Currency.ToUpperInvariant();
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.Tags = request.Tags?.ToArray() ?? Array.Empty<string>();
        product.UpdatedAtUtc = DateTime.UtcNow;
        product.PublishedAtUtc = request.IsActive ? product.PublishedAtUtc ?? DateTime.UtcNow : null;

        var saved = await _productRepository.UpdateAsync(product, cancellationToken);
        await _searchIndexer.PublishProductChangeAsync(saved.Id, cancellationToken);
        return saved.ToDto();
    }

    public async Task PublishIndexEventAsync(Guid productId, CancellationToken cancellationToken)
    {
        await _searchIndexer.PublishProductChangeAsync(productId, cancellationToken);
    }

    private async Task EnsureCategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        if (await _categoryRepository.GetByIdAsync(categoryId, cancellationToken) is null)
        {
            throw new KeyNotFoundException("Category does not exist");
        }
    }
}
