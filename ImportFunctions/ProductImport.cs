using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ImportFunctions;

public class ProductImport
{
    private readonly ILogger _logger;

    public ProductImport(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ProductImport>();
    }

    [Function("ProductImport")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"Product import started at {DateTime.Now}");

        try
        {
            //Fetch product data from ERP API (Stub) could be batched
            var productData = FetchFromERPApi();
            if (string.IsNullOrEmpty(productData))
            {
                _logger.LogWarning("No product data fetched from ERP API.");
                return;
            }

            PushProductData(productData);

            _logger.LogInformation("Product import completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during product import: {ex.Message}");
        }
        finally
        {
            _logger.LogInformation($"Product import finished at {DateTime.Now}");
        }
    }

    /// <summary>
    /// Fetches product data from a stubbed ERP API.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    private string FetchFromERPApi()
    {
        var fileName = "ProductData.json";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        if (!File.Exists(filePath))
        {
            _logger.LogError($"File not found: {filePath}");
            throw new FileNotFoundException($"Product data file not found: {filePath}");
        }

        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// Simulates pushing product data into a queue for further processing.
    /// </summary>
    /// <param name="productData">The product data to push.</param>
    private void PushProductData(string productData)
    {
        try
        {
            // Simulate queueing logic
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to push product data to queue: {ex.Message}");
        }
        _logger.LogInformation("Product data pushed for processing. Data length: {Length}", productData?.Length ?? 0);
    }
}
