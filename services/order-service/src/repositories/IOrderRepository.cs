using OrderService.Api.Domain.Entities;

namespace OrderService.Api.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Order> InsertAsync(Order order, CancellationToken cancellationToken = default);
}
