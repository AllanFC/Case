using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Repository.Models;

public class Product
{
    [Key]
    [MaxLength(25)]
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    [MaxLength(25)]
    [JsonPropertyName("category")]
    public required string CategoryId { get; set; }
    [JsonIgnore]
    public Category? Category { get; set; }
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("image")]
    public string? Image { get; set; }
}