using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using OrderService.Api.Domain.Entities;
using OrderService.Api.Repositories;
using OrderService.Api.Transport;

namespace OrderService.Api.Services;

public class OrderProcessingService : IOrderProcessingService
{
    private readonly IOrderRepository _repository;
    private readonly EventHubProducerClient? _producerClient;

    public OrderProcessingService(IOrderRepository repository, IEnumerable<EventHubProducerClient> producerClients)
    {
        _repository = repository;
        _producerClient = producerClients.FirstOrDefault();
    }

    public async Task<Order> PlaceOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = GenerateOrderNumber(),
            CustomerId = request.CustomerId,
            TotalAmount = request.Items.Sum(i => i.UnitPrice * i.Quantity),
            Items = request.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                Name = i.Name,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };

        var created = await _repository.InsertAsync(order, cancellationToken);
        await PublishDomainEventAsync(created, cancellationToken);
        return created;
    }

    private async Task PublishDomainEventAsync(Order order, CancellationToken cancellationToken)
    {
        if (_producerClient is null)
        {
            return;
        }

        var payload = JsonSerializer.Serialize(new
        {
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.Status,
            order.TotalAmount,
            order.CreatedAtUtc,
            Items = order.Items.Select(i => new { i.ProductId, i.Quantity, i.UnitPrice })
        });

        using var eventBatch = await _producerClient.CreateBatchAsync(cancellationToken);
        if (!eventBatch.TryAdd(new EventData(BinaryData.FromString(payload))))
        {
            throw new InvalidOperationException("Unable to enqueue order event");
        }

        await _producerClient.SendAsync(eventBatch, cancellationToken);
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
    }
}
