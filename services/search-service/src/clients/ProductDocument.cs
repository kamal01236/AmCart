using System.Text.Json.Serialization;

namespace SearchService.Api.Clients;

public record class ProductDocument
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("sku")]
    public string Sku { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; init; }

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = "USD";

    [JsonPropertyName("categoryId")]
    public Guid CategoryId { get; init; }

    [JsonPropertyName("categoryName")]
    public string CategoryName { get; init; } = string.Empty;

    [JsonPropertyName("tags")]
    public IReadOnlyCollection<string>? Tags { get; init; }

    [JsonPropertyName("rating")]
    public double? Rating { get; init; }

    [JsonPropertyName("reviewCount")]
    public int? ReviewCount { get; init; }

    [JsonPropertyName("popularity")]
    public int? Popularity { get; init; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; init; }

    [JsonPropertyName("publishedAtUtc")]
    public DateTime? PublishedAtUtc { get; init; }
}
