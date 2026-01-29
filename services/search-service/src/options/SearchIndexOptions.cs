using System.ComponentModel.DataAnnotations;

namespace SearchService.Api.Options;

public class SearchIndexOptions
{
    [Required]
    [Url]
    public string Endpoint { get; set; } = "https://localhost:9200/";

    [Required]
    public string IndexName { get; set; } = "products";

    public string? ApiKey { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
}
