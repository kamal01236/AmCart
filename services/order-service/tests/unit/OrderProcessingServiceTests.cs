using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OrderService.Api.Domain.Entities;
using OrderService.Api.Config;
using OrderService.Api.Repositories;
using OrderService.Api.Services;
using OrderService.Api.Transport;
using Xunit;

namespace OrderService.Api.Tests.Unit;

public class OrderProcessingServiceTests
{
    [Fact]
    public async Task PlaceOrderAsync_Creates_Order_And_Publishes_Event()
    {
        var repo = new Mock<IOrderRepository>();
        var producer = new Mock<IProducer<string, string>>();
        repo.Setup(r => r.InsertAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o);

        var options = Options.Create(new KafkaOptions
        {
            BootstrapServers = "localhost:9092",
            OrderTopic = "orders.v1"
        });

        var service = new OrderProcessingService(repo.Object, options, producer.Object, Mock.Of<ILogger<OrderProcessingService>>());
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid(),
            Items = new[]
            {
                new CreateOrderItemRequest
                {
                    ProductId = Guid.NewGuid(),
                    Name = "Test Product",
                    UnitPrice = 10,
                    Quantity = 2
                }
            }
        };

        var order = await service.PlaceOrderAsync(request, CancellationToken.None);
        order.CustomerId.Should().Be(request.CustomerId);
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(20);
        order.Status.Should().Be("Pending");

        producer.Verify(p => p.ProduceAsync("orders.v1", It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
