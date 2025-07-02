namespace Core.Models;

public class Category
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}