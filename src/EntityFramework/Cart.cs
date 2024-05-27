using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Cart")]
public class Cart
{
    public Guid CartId = Guid.NewGuid();
    // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserID { get; set; }
    public virtual User? User { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
}
