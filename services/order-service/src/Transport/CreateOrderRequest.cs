using System.ComponentModel.DataAnnotations;

namespace OrderService.Api.Transport;

public sealed class CreateOrderRequest
{
    [Required]
    public Guid CustomerId { get; init; }

    [Required]
    [MinLength(1)]
    public IEnumerable<CreateOrderItemRequest> Items { get; init; } = Array.Empty<CreateOrderItemRequest>();
}

public sealed class CreateOrderItemRequest
{
    [Required]
    public Guid ProductId { get; init; }

    [Required]
    [StringLength(256)]
    public string Name { get; init; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; init; }

    [Range(1, 999)]
    public int Quantity { get; init; }
}
