using System.ComponentModel.DataAnnotations;
using api.Dtos;
using Dtos.Product;

namespace Dtos.Cart
{
    public class CartDto
    {
        [Required(ErrorMessage = "User Id is required")]
        public Guid ProductID { get; set; }
        [Required(ErrorMessage = "User Id is required")]
        public Guid UserID { get; set; }
        public List<ProductDto> Products { get; set; } = []; // ! comment this first if there is any errors
        public UserDto? User { get; set; }
    }
}