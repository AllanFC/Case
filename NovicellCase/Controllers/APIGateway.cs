using System.Net.Http.Json;
using Core.Models.Products;
using Microsoft.AspNetCore.Mvc;

namespace NovicellCase.Controllers;

[ApiController]
[Route("api/[controller]")]
public class APIGateway : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public APIGateway(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // GET: api/apigateway/products/{id}
    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProductById(string id, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var baseUrl = _configuration["ProductsMicroservice:BaseUrl"];
        var response = await client.GetAsync($"{baseUrl}/api/products/{id}", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
            return Ok(product);
        }

        return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
    }

    // POST: api/apigateway/products/filter
    [HttpPost("products/filter")]
    public async Task<IActionResult> GetProducts([FromBody] ProductFilterDto filter, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var baseUrl = _configuration["ProductsMicroservice:BaseUrl"];
        var response = await client.PostAsJsonAsync($"{baseUrl}/api/products/filter", filter, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
            return Ok(result);
        }

        return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
    }
}
