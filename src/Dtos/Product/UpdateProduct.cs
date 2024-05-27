namespace Dtos.Product
{
    public class UpdateProductDto
    {
        public required string ProductName { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}