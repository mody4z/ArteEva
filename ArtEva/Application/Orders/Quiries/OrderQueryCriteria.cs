using ArtEva.Application.Enum;
using ArtEva.Models.Enums;

namespace ArtEva.Application.Orders.Quiries
{
    public class OrderQueryCriteria
    {
        public int? SellerUserId { get; set; }
        public int? BuyerUserId { get; set; }

        public int? ShopId { get; set; }
        public OrderStatus? Status { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }

        public string? OrderNumber { get; set; }

        public OrderSortBy SortBy { get; set; } = OrderSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}
