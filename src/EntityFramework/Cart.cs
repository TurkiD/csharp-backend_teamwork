using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Cart")]
public class Cart
{
    public Guid CartId = Guid.NewGuid();
    // [Required(ErrorMessage = "User Id is required")]
    [Required(ErrorMessage = "User Id is required")]
    // public Guid ProductID { get; set; }
    public Guid UserID { get; set; }
    // public virtual Product? Product { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
    public virtual User? User { get; set; }
}
