using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SearchService.Api.Options;
using SearchService.Api.Services;

namespace SearchService.Api.Clients;

public class ElasticSearchIndexClient : ISearchIndexClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private readonly HttpClient _httpClient;
    private readonly SearchIndexOptions _options;
    private readonly ILogger<ElasticSearchIndexClient> _logger;

    public ElasticSearchIndexClient(HttpClient httpClient, IOptions<SearchIndexOptions> options, ILogger<ElasticSearchIndexClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<SearchResult<ProductDocument>> SearchAsync(SearchQuery query, CancellationToken cancellationToken)
    {
        var payload = BuildRequest(query);
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.IndexName}/_search")
        {
            Content = JsonContent.Create(payload, options: SerializerOptions)
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Search index error {StatusCode}: {Body}", response.StatusCode, body);
            response.EnsureSuccessStatusCode();
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var elasticResponse = await JsonSerializer.DeserializeAsync<ElasticSearchResponse>(stream, SerializerOptions, cancellationToken);

        if (elasticResponse?.Hits?.Hits is null)
        {
            return new SearchResult<ProductDocument>(0, Array.Empty<SearchDocument<ProductDocument>>());
        }

        var documents = elasticResponse.Hits.Hits
            .Select(hit => new SearchDocument<ProductDocument>(hit.Source ?? new ProductDocument(), hit.Score))
            .ToArray();

        var total = elasticResponse.Hits.Total?.Value ?? documents.Length;
        return new SearchResult<ProductDocument>(total, documents);
    }

    private static object BuildRequest(SearchQuery query)
    {
        var mustClauses = new List<object>();
        if (!string.IsNullOrWhiteSpace(query.Query))
        {
            mustClauses.Add(new
            {
                multi_match = new
                {
                    query = query.Query,
                    fields = new[] { "name^3", "description", "tags^2" },
                    fuzziness = "AUTO",
                    @operator = "and"
                }
            });
        }
        else
        {
            mustClauses.Add(new { match_all = new { } });
        }

        var filterClauses = new List<object>();
        if (query.CategoryId is Guid categoryId)
        {
            filterClauses.Add(new { term = new { categoryId } });
        }

        if (query.MinPrice.HasValue || query.MaxPrice.HasValue)
        {
            var priceRange = new Dictionary<string, object>();
            if (query.MinPrice.HasValue)
            {
                priceRange["gte"] = query.MinPrice.Value;
            }

            if (query.MaxPrice.HasValue)
            {
                priceRange["lte"] = query.MaxPrice.Value;
            }

            filterClauses.Add(new { range = new { price = priceRange } });
        }

        if (query.Tags.Count > 0)
        {
            filterClauses.Add(new { terms = new { tags = query.Tags } });
        }

        var boolQuery = new Dictionary<string, object>
        {
            ["must"] = mustClauses
        };

        if (filterClauses.Count > 0)
        {
            boolQuery["filter"] = filterClauses;
        }

        var request = new Dictionary<string, object?>
        {
            ["from"] = query.Page * query.PageSize,
            ["size"] = query.PageSize,
            ["query"] = new { @bool = boolQuery }
        };

        if (BuildSortClause(query.Sort) is { } sortClause)
        {
            request["sort"] = sortClause;
        }

        return request;
    }

    private static object? BuildSortClause(SearchSortOption sort)
    {
        return sort switch
        {
            SearchSortOption.PriceAsc => new object[] { new { price = new { order = "asc" } } },
            SearchSortOption.PriceDesc => new object[] { new { price = new { order = "desc" } } },
            SearchSortOption.Newest => new object[] { new { publishedAtUtc = new { order = "desc" } } },
            SearchSortOption.Popularity => new object[] { new { popularity = new { order = "desc" } } },
            _ => null
        };
    }

    private sealed record ElasticSearchResponse(
        [property: JsonPropertyName("hits")] ElasticHits? Hits);

    private sealed record ElasticHits(
        [property: JsonPropertyName("total")] ElasticTotal? Total,
        [property: JsonPropertyName("hits")] IReadOnlyList<ElasticHit>? Hits);

    private sealed record ElasticTotal(
        [property: JsonPropertyName("value")] long Value);

    private sealed record ElasticHit(
        [property: JsonPropertyName("_id")] string? Id,
        [property: JsonPropertyName("_score")] double? Score,
        [property: JsonPropertyName("_source")] ProductDocument? Source);
}
