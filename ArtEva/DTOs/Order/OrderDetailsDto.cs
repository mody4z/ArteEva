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
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TaxTotal { get; set; }

        public int? ExecutionDays { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string ProductTitle { get; set; }
        public string? ProductImage { get; set; }
        public CancellationInfoDto? Cancellation { get; set; }

    }
}
