using System;
using System.Collections.Generic;
using FluentAssertions;
using OrderService.Api.Domain.Entities;
using OrderService.Api.Services;
using Xunit;

namespace OrderService.Api.Tests.Unit;

public class OrderMappingExtensionsTests
{
    [Fact]
    public void ToDto_Maps_Order_And_Items()
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-20260129-XYZ",
            CustomerId = Guid.NewGuid(),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            Status = "Pending",
            TotalAmount = 123.45m,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Name = "Product 1",
                    UnitPrice = 10,
                    Quantity = 2
                },
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Name = "Product 2",
                    UnitPrice = 20,
                    Quantity = 5
                }
            }
        };

        var dto = order.ToDto();
        dto.Id.Should().Be(order.Id);
        dto.OrderNumber.Should().Be(order.OrderNumber);
        dto.CustomerId.Should().Be(order.CustomerId);
        dto.Status.Should().Be(order.Status);
        dto.TotalAmount.Should().Be(order.TotalAmount);
        dto.Items.Should().HaveCount(2);
        var items = dto.Items.ToArray();
        items[0].Name.Should().Be("Product 1");
        items[1].LineTotal.Should().Be(100);
    }
}
