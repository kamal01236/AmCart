using CatalogService.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _dbContext;

    public ProductRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Products.Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        return await _dbContext.Products.Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> SearchAsync(string? query, Guid? categoryId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var products = _dbContext.Products.Include(p => p.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim().ToLowerInvariant();
            products = products.Where(p => p.Name.ToLower().Contains(normalized) || p.Description.ToLower().Contains(normalized) || p.Sku.ToLower().Contains(normalized));
        }

        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        return await products
            .OrderByDescending(p => p.PublishedAtUtc)
            .Skip(page * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return product;
    }
}
