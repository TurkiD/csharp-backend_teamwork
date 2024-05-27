using Dtos.Order;
using Dtos.Product;
using Dtos.User;

namespace Dtos.Orders
{
    public class OrderDto
    {
        public required Guid OrderId { get; set; }
        public required OrderStatus Status { get; set; } = OrderStatus.Pending;
        public required PaymentMethod Payment { get; set; } = PaymentMethod.CreditCard;
        public required double Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }


        public UserInfoForOrderDto? User { get; set; }
        public List<ProductDto>? Products { get; set; }
    }
}