using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderService.Api.Config;
using OrderService.Api.Domain.Entities;
using OrderService.Api.Events;
using OrderService.Api.Repositories;
using OrderService.Api.Transport;

namespace OrderService.Api.Services;

public class OrderProcessingService : IOrderProcessingService
{
    private readonly IOrderRepository _repository;
    private readonly IProducer<string, string> _producer;
    private readonly KafkaOptions _kafkaOptions;
    private readonly ILogger<OrderProcessingService> _logger;

    public OrderProcessingService(
        IOrderRepository repository,
        IOptions<KafkaOptions> kafkaOptions,
        IProducer<string, string> producer,
        ILogger<OrderProcessingService> logger)
    {
        _repository = repository;
        _kafkaOptions = kafkaOptions.Value;
        _producer = producer;
        _logger = logger;
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
        var evt = new OrderPlacedEvent(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.TotalAmount,
            order.Status,
            order.CreatedAtUtc,
            order.Items.Select(i => new OrderPlacedItem(i.ProductId, i.Quantity, i.UnitPrice)).ToArray());

        var payload = JsonSerializer.Serialize(evt);
        var message = new Message<string, string>
        {
            Key = order.Id.ToString(),
            Value = payload,
            Headers = new Headers
            {
                new Header("ce_type", Encoding.UTF8.GetBytes("OrderPlaced")),
                new Header("ce_source", Encoding.UTF8.GetBytes("order-service")),
                new Header("ce_specversion", Encoding.UTF8.GetBytes("1.0"))
            }
        };

        try
        {
            await _producer.ProduceAsync(_kafkaOptions.OrderTopic, message, cancellationToken);
            _logger.LogInformation("Published order event to topic {Topic} for order {OrderId}", _kafkaOptions.OrderTopic, order.Id);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to publish order event for {OrderId}", order.Id);
            throw new InvalidOperationException("Unable to enqueue order event", ex);
        }
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
    }
}
