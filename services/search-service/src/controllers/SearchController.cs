using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SearchService.Api.Services;
using SearchService.Api.Transport;

namespace SearchService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/search/products")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProductSearchResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] ProductSearchRequest request, CancellationToken cancellationToken)
    {
        var response = await _searchService.SearchAsync(request, cancellationToken);
        return Ok(response);
    }
}
