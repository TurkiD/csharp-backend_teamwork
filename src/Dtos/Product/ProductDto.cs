using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dtos.Category;

namespace Dtos.Product
{
    public class ProductDto
    {
        public Guid ProductID { get; set; } = Guid.NewGuid();
        public required string ProductName { get; set; }
        public string Description { get; set; } = string.Empty;
        public required int Quantity { get; set; }
        public required decimal Price { get; set; }
        public required Guid CategoryID { get; set; }
        // public CategoryDto? Category { get; set; }
        // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}