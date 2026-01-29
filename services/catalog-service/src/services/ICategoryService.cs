using CatalogService.Api.Transport;

namespace CatalogService.Api.Services;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<CategoryDto> CreateAsync(UpsertCategoryRequest request, CancellationToken cancellationToken);
    Task<CategoryDto> UpdateAsync(Guid id, UpsertCategoryRequest request, CancellationToken cancellationToken);
}
