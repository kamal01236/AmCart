namespace SearchService.Api.Services;

public enum SearchSortOption
{
    Relevance = 0,
    PriceAsc,
    PriceDesc,
    Newest,
    Popularity
}

public static class SearchSortOptionParser
{
    public static SearchSortOption Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return SearchSortOption.Relevance;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "price" or "price_asc" or "price-asc" => SearchSortOption.PriceAsc,
            "price_desc" or "price-desc" => SearchSortOption.PriceDesc,
            "new" or "newest" or "date_desc" => SearchSortOption.Newest,
            "popular" or "popularity" => SearchSortOption.Popularity,
            _ => SearchSortOption.Relevance
        };
    }
}
