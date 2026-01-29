using SearchService.Api.Services;

namespace SearchService.Api.Clients;

public interface ISearchIndexClient
{
    Task<SearchResult<ProductDocument>> SearchAsync(SearchQuery query, CancellationToken cancellationToken);
}
