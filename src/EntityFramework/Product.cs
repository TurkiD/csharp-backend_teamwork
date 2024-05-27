using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFramework;

[Table("Products")]
public class Product
{
    public Guid ProductID { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(50)]
    public string ProductName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required")]
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [Required(ErrorMessage = "CategoryId is required")]
    public Guid CategoryId { get; set; }

    public virtual Category? Category { get; set; }


    public List<Cart> Carts { set; get; } = [];
    public List<Order> Orders { get; set; } = new List<Order>();
}
