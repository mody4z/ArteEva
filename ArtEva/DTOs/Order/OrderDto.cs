// ArtEva.DTOs.Orders
using ArtEva.Models.Enums;

namespace ArtEva.DTOs.Orders
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal GrandTotal { get; set; }
        public int ShopId { get; set; }
        public int BuyerId { get; set; }
        public OrderStatus Status { get; set; }
        public int? ExecutionDays { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
