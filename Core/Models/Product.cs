namespace Core.Models;

public class Product
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required Category Category { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
}