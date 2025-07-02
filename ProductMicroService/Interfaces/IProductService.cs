using Repository.Models;
using Core.Models.Products;

namespace ProductMicroService.Interfaces;

public interface IProductService
{
    Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<(IEnumerable<Product> Items, int TotalItems)> GetProductsAsync(
        ProductFilterDto filter,
        CancellationToken cancellationToken = default);
}
