using CatalogService.Api.Domain;
using CatalogService.Api.Repositories;
using CatalogService.Api.Transport;

namespace CatalogService.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var categories = await _repository.GetAllAsync(cancellationToken);
        return categories.Select(c => c.ToDto()).ToArray();
    }

    public async Task<CategoryDto> CreateAsync(UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = new Category
        {
            Name = request.Name,
            Slug = request.Slug.ToLowerInvariant(),
            Description = request.Description,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var saved = await _repository.CreateAsync(entity, cancellationToken);
        return saved.ToDto();
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken) ??
                       throw new KeyNotFoundException("Category not found");

        existing.Name = request.Name;
        existing.Slug = request.Slug.ToLowerInvariant();
        existing.Description = request.Description;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        var saved = await _repository.UpdateAsync(existing, cancellationToken);
        return saved.ToDto();
    }
}
