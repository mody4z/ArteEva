using ArtEva.Models.Enums;

namespace ArtEva.DTOs.Order
{
    public class OrderListBuyerDto
    {
        public int OrderId { get; init; }
        public string OrderNumber { get; init; } = null!;
        public OrderStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public decimal GrandTotal { get; init; }

    }
}
