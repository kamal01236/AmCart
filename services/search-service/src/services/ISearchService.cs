using SearchService.Api.Transport;

namespace SearchService.Api.Services;

public interface ISearchService
{
    Task<ProductSearchResponse> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken);
}
