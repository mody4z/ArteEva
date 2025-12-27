namespace ArtEva.DTOs.Order
{
    public class CreateOrderFromCartItemDto
    {
        public int CartItemId { get; init; }
        public int UserId { get; init; }

        public int ProductId { get; init; }
        public int ShopId { get; init; }

        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal Subtotal { get; init; }

        public string ProductTitle { get; init; } = null!;
        public string? ProductImage { get; init; }

        public int? ExecutionDays { get; init; }

        public bool IsConvertedToOrder { get; init; }
    }
}
