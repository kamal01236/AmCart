using CatalogService.Api.Domain;
using CatalogService.Api.Repositories;
using CatalogService.Api.Services;
using CatalogService.Api.Transport;
using FluentAssertions;
using Moq;
using Xunit;

namespace CatalogService.Api.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();
    private readonly Mock<ISearchIndexer> _searchIndexer = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_productRepo.Object, _categoryRepo.Object, _searchIndexer.Object);
    }

    [Fact]
    public async Task CreateAsync_Saves_Product_And_Publishes_Event()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category { Id = categoryId, Name = "Shoes", Slug = "shoes" });

        _productRepo.Setup(r => r.GetBySkuAsync("SKU-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _productRepo.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var request = new UpsertProductRequest("SKU-1", "Sneaker", "Lightweight", 99.99m, "USD", categoryId, true, new[] { "running" });

        var result = await _service.CreateAsync(request, CancellationToken.None);

        result.Sku.Should().Be("SKU-1");
        _searchIndexer.Verify(i => i.PublishProductChangeAsync(result.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Throws_When_Product_Not_Found()
    {
        _productRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var request = new UpsertProductRequest("SKU-1", "Sneaker", "Lightweight", 99.99m, "USD", Guid.NewGuid(), true, Array.Empty<string>());

        var act = async () => await _service.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
