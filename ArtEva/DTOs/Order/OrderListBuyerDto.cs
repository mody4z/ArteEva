using ArtEva.Models.Enums;

namespace ArtEva.DTOs.Order
{
    public class OrderListBuyerDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal GrandTotal { get; set; }

        public string ProductTitle { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }


    }
}
