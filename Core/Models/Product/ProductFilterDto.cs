namespace Core.Models.Products
{
    public class ProductFilterDto
    {
        public string? CategoryId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}