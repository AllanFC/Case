using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.Models;

namespace ImportFunctions;

public class ProductConsumer
{
    private readonly ILogger _logger;
    private readonly DbContext _dbContext;

    public ProductConsumer(ILoggerFactory loggerFactory, DbContext dbContext)
    {
        _logger = loggerFactory.CreateLogger<ProductConsumer>();
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    // Should have been a service bus queue trigger but for time management we use a timer trigger
    [Function("ProductConsumer")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"Product consumption started at {DateTime.Now}");
        try
        {
            // Simulate consuming product data from a queue or other source
            var productData = ConsumeProductData();
            if (string.IsNullOrEmpty(productData))
            {
                _logger.LogWarning("No product data to consume.");
                return;
            }

            ProcessProductData(productData);
            _logger.LogInformation("Product consumption completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during product consumption: {ex.Message}");
        }
        finally
        {
            _logger.LogInformation($"Product consumption finished at {DateTime.Now}");
        }
    }

    private string ConsumeProductData()
    {
        // From queue
        var fileName = "ProductData.json";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        if (!File.Exists(filePath))
        {
            _logger.LogError($"File not found: {filePath}");
            throw new FileNotFoundException($"Product data file not found: {filePath}");
        }

        return File.ReadAllText(filePath);
    }

    private void ProcessProductData(string productData)
    {
        List<Repository.Models.Product>? products;
        try
        {
            products = System.Text.Json.JsonSerializer.Deserialize<List<Repository.Models.Product>>(productData);
            if (products == null || products.Count == 0)
            {
                _logger.LogWarning("No valid products found in the provided data.");
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to deserialize product data: {ex.Message}");
            return;
        }

        // Ensure all categories exist
        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var existingCategoryIds = _dbContext.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToHashSet();

        var missingCategoryIds = categoryIds.Except(existingCategoryIds).ToList();
        if (missingCategoryIds.Any())
        {
            _logger.LogWarning($"Missing categories for IDs: {string.Join(", ", missingCategoryIds)}. Products with these categories will be skipped.");
        }

        // Filter products to only those with existing categories
        var validProducts = products.Where(p => existingCategoryIds.Contains(p.CategoryId)).ToList();

        if (!validProducts.Any())
        {
            _logger.LogWarning("No products with valid categories to save.");
            return;
        }

        foreach (var product in validProducts)
        {
            var existing = _dbContext.Products.Find(product.Id);
            if (existing == null)
            {
                _dbContext.Products.Add(product);
            }
            else
            {
                _dbContext.Entry(existing).CurrentValues.SetValues(product);
            }
        }
        _dbContext.SaveChanges();
        _logger.LogInformation($"{validProducts.Count} products processed and saved to the database.");
    }

}
