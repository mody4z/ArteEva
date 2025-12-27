using ArtEva.Models.Enums;

namespace ArtEva.DTOs.Order
{
    public class OrderDetailsDto
    {
        public int OrderId { get; init; }
        public int BuyerUserId { get; init; }
        public int SellerUserId { get; init; }

        public string OrderNumber { get; init; } = null!;
        public OrderStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }

        public decimal GrandTotal { get; init; }
    }
}
