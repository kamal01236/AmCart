using CatalogService.Api.Domain;
using CatalogService.Api.Repositories;
using CatalogService.Api.Services;
using CatalogService.Api.Transport;
using FluentAssertions;
using Moq;
using Xunit;

namespace CatalogService.Api.Tests;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repository = new();
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _service = new CategoryService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_Persists_Category()
    {
        var request = new UpsertCategoryRequest("Shoes", "shoes", "Footwear");
        _repository.Setup(r => r.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        var result = await _service.CreateAsync(request, CancellationToken.None);

        result.Name.Should().Be("Shoes");
        _repository.Verify(r => r.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
