namespace Dtos.Order
{
    public enum OrderStatus { Creating = 0, Pending = 1, Processing = 2, Shipped = 3, Delivered = 4 };
    public enum PaymentMethod { CreditCard = 0, ApplePay = 1, Visa = 2, Cash = 3, PayPal = 4 };
    public class UpdateOrderDto
    {
        public required OrderStatus Status { get; set; }
        public required PaymentMethod Payment { get; set; }
    }
}