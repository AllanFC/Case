using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ImportFunctions;

public class CategoryImport
{
    private readonly ILogger _logger;

    public CategoryImport(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CategoryImport>();
    }

    [Function("CategoryImport")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"Category import started at {DateTime.Now}");

        try
        {
            //Fetch category data from ERP API could be batched
            var categoryData = FetchFromERPApi();
            if (string.IsNullOrEmpty(categoryData))
            {
                _logger.LogWarning("No produCategoryct data fetched from ERP API.");
                return;
            }

            PushCategoryData(categoryData);

            _logger.LogInformation("Category import completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during category import: {ex.Message}");
        }
        finally
        {
            _logger.LogInformation($"Category import finished at {DateTime.Now}");
        }
    }

    /// <summary>
    /// Fetches category data from a stubbed ERP API.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    private string FetchFromERPApi()
    {
        var fileName = "CategoryData.json";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        if (!File.Exists(filePath))
        {
            _logger.LogError($"File not found: {filePath}");
            throw new FileNotFoundException($"Category data file not found: {filePath}");
        }

        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// Simulates placing category data onto a queue for further processing.
    /// </summary>
    /// <param name="categoryData">The category data to push.</param>
    private void PushCategoryData(string categoryData)
    {
        try
        {
            // Simulate queueing logic
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to push category data to queue: {ex.Message}");
        }
        _logger.LogInformation("Category data enqueued for processing. Data length: {Length}", categoryData?.Length ?? 0);
    }
}
