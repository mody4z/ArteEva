using ArtEva.Models.Enums;

namespace ArtEva.DTOs.Order
{
    public class OrderForSellerActionDto
    {
        public int OrderId { get; set; }
        public int ShopId { get; set; }
        public OrderStatus Status { get; set; }
        // Useful extras for Seller flow
        public int? ExecutionDays { get; set; }
        public int BuyerUserId { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
