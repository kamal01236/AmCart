using OrderService.Api.Domain.Entities;
using OrderService.Api.Transport;

namespace OrderService.Api.Services;

public interface IOrderProcessingService
{
    Task<Order> PlaceOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
}
