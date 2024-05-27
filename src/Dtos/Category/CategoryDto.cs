using Dtos.Product;

namespace Dtos.Category
{
    public class CategoryDto
    {
        public Guid CategoryID { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // public List<ProductDto> Products { get; set; } = [];
    }
}