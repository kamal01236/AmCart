using CatalogService.Api.Domain;

namespace CatalogService.Api.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken);
    Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken);
}
