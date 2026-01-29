using CatalogService.Api.Services;
using CatalogService.Api.Transport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/catalog/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] ProductSearchRequest request, CancellationToken cancellationToken)
    {
        var products = await _service.SearchAsync(request, cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _service.GetAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Catalog.Admin")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] UpsertProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Catalog.Admin")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(product);
    }

    [HttpPost("{id:guid}/index")]
    [Authorize(Roles = "Catalog.Admin")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Reindex(Guid id, CancellationToken cancellationToken)
    {
        await _service.PublishIndexEventAsync(id, cancellationToken);
        return Accepted();
    }
}
