using System.Text.Json;
using Azure.Messaging.EventHubs.Producer;

namespace CatalogService.Api.Services;

public class EventHubSearchIndexer : ISearchIndexer
{
    private readonly EventHubProducerClient _producerClient;

    public EventHubSearchIndexer(EventHubProducerClient producerClient)
    {
        _producerClient = producerClient;
    }

    public async Task PublishProductChangeAsync(Guid productId, CancellationToken cancellationToken)
    {
        using var eventBatch = await _producerClient.CreateBatchAsync(cancellationToken);
        var payload = JsonSerializer.SerializeToUtf8Bytes(new
        {
            type = "ProductChanged",
            productId,
            occurredAtUtc = DateTime.UtcNow
        });
        if (!eventBatch.TryAdd(new Azure.Messaging.EventHubs.EventData(payload)))
        {
            throw new InvalidOperationException("Failed to enqueue product change event");
        }

        await _producerClient.SendAsync(eventBatch, cancellationToken);
    }
}
