using System;
using System.Collections.Generic;

namespace OrderService.Api.Events;

public record OrderPlacedEvent(
    Guid OrderId,
    string OrderNumber,
    Guid CustomerId,
    decimal TotalAmount,
    string Status,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyCollection<OrderPlacedItem> Items);

public record OrderPlacedItem(Guid ProductId, int Quantity, decimal UnitPrice);
