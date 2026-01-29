namespace CatalogService.Api.Services;

public interface ISearchIndexer
{
    Task PublishProductChangeAsync(Guid productId, CancellationToken cancellationToken);
}
