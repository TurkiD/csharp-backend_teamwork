using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dtos.Product
{
    public class ProductOrderDto
    {
        public Guid ProductID { get; set; } = Guid.NewGuid();
        public required string ProductName { get; set; }
        public required decimal Price { get; set; }
    }
}