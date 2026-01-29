using System;
using System.Collections.Generic;

namespace OrderService.Api.Transport;

public record OrderDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    DateTimeOffset CreatedAtUtc,
    string Status,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items);

public record OrderItemDto(
    Guid ProductId,
    string Name,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);
