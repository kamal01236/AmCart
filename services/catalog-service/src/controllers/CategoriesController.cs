using CatalogService.Api.Services;
using CatalogService.Api.Transport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/catalog/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyCollection<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _service.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    [Authorize(Roles = "Catalog.Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Catalog.Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(category);
    }
}
