using Core.Models.Products;
using ProductMicroService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;

namespace ProductMicroService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(string id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetProductByIdAsync(id, cancellationToken);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult> GetProducts([FromBody] ProductFilterDto filter, CancellationToken cancellationToken)
    {
        if (filter.Page <= 0 || filter.PageSize <= 0)
            return BadRequest("Page and PageSize must be greater than zero.");

        var (items, totalItems) = await _productService.GetProductsAsync(filter, cancellationToken);

        var result = new
        {
            TotalItems = totalItems,
            Page = filter.Page,
            PageSize = filter.PageSize,
            Items = items
        };

        return Ok(result);
    }
}
