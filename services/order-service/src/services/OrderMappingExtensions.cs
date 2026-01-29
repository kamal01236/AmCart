using OrderService.Api.Domain.Entities;
using OrderService.Api.Transport;

namespace OrderService.Api.Services;

public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.CreatedAtUtc,
            order.Status,
            order.TotalAmount,
            order.Items.Select(i => i.ToDto()).ToArray()
        );
    }

    public static OrderItemDto ToDto(this OrderItem item)
    {
        return new OrderItemDto(
            item.ProductId,
            item.Name,
            item.UnitPrice,
            item.Quantity,
            item.LineTotal
        );
    }
}
