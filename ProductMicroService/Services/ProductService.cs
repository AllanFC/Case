using Core.Models.Products;
using Microsoft.EntityFrameworkCore;
using ProductMicroService.Interfaces;
using Repository;
using Repository.Models;

namespace ProductMicroService.Services;

public class ProductService : IProductService
{
    private readonly Repository.DbContext _dbContext;

    public ProductService(Repository.DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<Product> Items, int TotalItems)> GetProductsAsync(
        ProductFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.CategoryId))
            query = query.Where(p => p.CategoryId == filter.CategoryId);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }
}
