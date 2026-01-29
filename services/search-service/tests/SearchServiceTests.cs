using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SearchService.Api.Clients;
using SearchService.Api.Services;
using SearchService.Api.Transport;
using Xunit;

namespace SearchService.Api.Tests;

public class SearchServiceTests
{
    private readonly Mock<ISearchIndexClient> _client = new();
    private readonly SearchService.Api.Services.SearchService _service;

    public SearchServiceTests()
    {
        _service = new SearchService.Api.Services.SearchService(_client.Object, Mock.Of<ILogger<SearchService.Api.Services.SearchService>>());
    }

    [Fact]
    public async Task SearchAsync_Normalizes_Request_And_Returns_Response()
    {
        SearchQuery? capturedQuery = null;
        var productId = Guid.NewGuid();

        _client.Setup(c => c.SearchAsync(It.IsAny<SearchQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchResult<ProductDocument>(1, new[]
            {
                new SearchDocument<ProductDocument>(new ProductDocument
                {
                    Id = productId,
                    Sku = "SKU-1",
                    Name = "Sneaker",
                    Description = "Lightweight shoe",
                    Price = 120,
                    Currency = "USD",
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "Shoes",
                    Tags = new[] { "running" },
                    Rating = 4.7,
                    ReviewCount = 120,
                    Popularity = 10,
                    ImageUrl = "https://example.com/shoe.jpg",
                    PublishedAtUtc = DateTime.UtcNow
                }, 1.1)
            }))
            .Callback<SearchQuery, CancellationToken>((query, _) => capturedQuery = query);

        var request = new ProductSearchRequest(
            "  Sneaker  ",
            Guid.NewGuid(),
            50,
            200,
            new[] { "Running", "running", "" },
            "price_desc",
            -1,
            500);

        var response = await _service.SearchAsync(request, CancellationToken.None);

        response.Page.Should().Be(0);
        response.PageSize.Should().Be(100);
        response.Total.Should().Be(1);
        response.Results.Should().HaveCount(1);
        response.Results.First().Id.Should().Be(productId);

        capturedQuery.Should().NotBeNull();
        capturedQuery!.Page.Should().Be(0);
        capturedQuery.PageSize.Should().Be(100);
        capturedQuery.Sort.Should().Be(SearchSortOption.PriceDesc);
        capturedQuery.Tags.Should().ContainSingle(tag => tag.Equals("Running", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchAsync_Throws_When_MinPrice_Exceeds_MaxPrice()
    {
        var request = new ProductSearchRequest(null, null, 250, 100, null, null, 0, 10);

        var act = async () => await _service.SearchAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
