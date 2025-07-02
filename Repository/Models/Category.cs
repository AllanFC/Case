using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Repository.Models;

public class Category
{
    [Key]
    [MaxLength(25)]
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [MaxLength(100)]
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}