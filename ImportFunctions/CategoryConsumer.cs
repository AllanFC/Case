using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.Models;

namespace ImportFunctions;

public class CategoryConsumer
{
    private readonly ILogger _logger;
    private readonly DbContext _dbContext;

    public CategoryConsumer(ILoggerFactory loggerFactory, DbContext dbContext)
    {
        _logger = loggerFactory.CreateLogger<CategoryConsumer>();
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    // Should have been a service bus queue trigger but for time management we use a timer trigger
    [Function("CategoryConsumer")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"Category consumption started at {DateTime.Now}");
        try
        {
            // Simulate consuming category data from a queue or other source
            var categoryData = ConsumeCategoryData();
            if (string.IsNullOrEmpty(categoryData))
            {
                _logger.LogWarning("No category data to consume.");
                return;
            }

            ProcessCategoryData(categoryData);
            _logger.LogInformation("Category consumption completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during category consumption: {ex.Message}");
        }
        finally
        {
            _logger.LogInformation($"Category consumption finished at {DateTime.Now}");
        }
    }

    private string ConsumeCategoryData()
    {
        // From queue
        var fileName = "CategoryData.json";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        if (!File.Exists(filePath))
        {
            _logger.LogError($"File not found: {filePath}");
            throw new FileNotFoundException($"Category data file not found: {filePath}");
        }

        return File.ReadAllText(filePath);
    }

    private void ProcessCategoryData(string categoryData)
    {
        List<Category>? categorys;
        try
        {
            categorys = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(categoryData);
            if (categorys == null || categorys.Count == 0)
            {
                _logger.LogWarning("No valid categorys found in the provided data.");
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to deserialize category data: {ex.Message}");
            return;
        }

        foreach (var category in categorys)
        {
            var existing = _dbContext.Categories.Find(category.Id);
            if (existing == null)
            {
                _dbContext.Categories.Add(category);
            }
            else
            {
                _dbContext.Entry(existing).CurrentValues.SetValues(category);
            }
        }
        _dbContext.SaveChanges();
        _logger.LogInformation($"{categorys.Count} categorys processed and saved to the database.");
    }
}
