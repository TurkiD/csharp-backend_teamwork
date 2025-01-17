using System.ComponentModel.DataAnnotations.Schema;
using Dtos.Order;

namespace EntityFramework
{
    [Table("Order")]
    public class Order
    {
        public required Guid OrderId { get; set; }
        public required OrderStatus Status { get; set; } = OrderStatus.Pending;
        public required PaymentMethod Payment { get; set; } = PaymentMethod.CreditCard;
        public required double Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Guid UserId { get; set; }
        // public Guid ProductId { get; set; }
        
        public virtual User? User { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}