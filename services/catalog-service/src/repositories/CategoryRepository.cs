using CatalogService.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CatalogDbContext _dbContext;

    public CategoryRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .OrderBy(c => c.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken)
    {
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return category;
    }
}
