using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Repositories;
using OrderService.Api.Services;
using OrderService.Api.Transport;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _repository;
    private readonly IOrderProcessingService _orderProcessingService;

    public OrdersController(IOrderRepository repository, IOrderProcessingService orderProcessingService)
    {
        _repository = repository;
        _orderProcessingService = orderProcessingService;
    }

    [HttpGet("{orderNumber}")]
    public async Task<IActionResult> GetByNumber(string orderNumber, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByNumberAsync(orderNumber, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetByCustomerAsync(customerId, cancellationToken);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var order = await _orderProcessingService.PlaceOrderAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByNumber), new { orderNumber = order.OrderNumber }, order);
    }
}
