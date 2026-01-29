using System.ComponentModel.DataAnnotations;
using SearchService.Api.Clients;
using SearchService.Api.Transport;

namespace SearchService.Api.Services;

public class SearchService : ISearchService
{
    private readonly ISearchIndexClient _indexClient;
    private readonly ILogger<SearchService> _logger;

    public SearchService(ISearchIndexClient indexClient, ILogger<SearchService> logger)
    {
        _indexClient = indexClient;
        _logger = logger;
    }

    public async Task<ProductSearchResponse> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var normalizedPage = Math.Max(request.Page, 0);
        var normalizedSize = Math.Clamp(request.PageSize, 1, 100);
        var normalizedQuery = request.Query?.Trim();
        var normalizedTags = NormalizeTags(request.Tags);
        var sortOption = SearchSortOptionParser.Parse(request.Sort);

        var searchQuery = new SearchQuery(
            normalizedQuery,
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            normalizedTags,
            sortOption,
            normalizedPage,
            normalizedSize);

        _logger.LogInformation(
            "Executing search query {@Query} page {Page} size {Size}",
            searchQuery,
            normalizedPage,
            normalizedSize);

        var result = await _indexClient.SearchAsync(searchQuery, cancellationToken);

        var response = new ProductSearchResponse(
            result.Total,
            normalizedPage,
            normalizedSize,
            result.Documents.Select(Map).ToArray());

        return response;
    }

    private static void ValidateRequest(ProductSearchRequest request)
    {
        if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
        {
            throw new ValidationException("MinPrice cannot be greater than MaxPrice.");
        }
    }

    private static IReadOnlyCollection<string> NormalizeTags(IReadOnlyCollection<string>? tags)
    {
        if (tags is null || tags.Count == 0)
        {
            return Array.Empty<string>();
        }

        return tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static ProductSearchResultItem Map(SearchDocument<ProductDocument> document)
    {
        var source = document.Document;
        return new ProductSearchResultItem(
            source.Id,
            source.Sku,
            source.Name,
            source.Description,
            source.Price,
            source.Currency,
            source.CategoryId,
            source.CategoryName,
            source.Tags ?? Array.Empty<string>(),
            document.Score,
            source.Rating,
            source.ReviewCount,
            source.Popularity,
            source.ImageUrl,
            source.PublishedAtUtc);
    }
}
