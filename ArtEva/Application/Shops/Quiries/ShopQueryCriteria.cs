using ArtEva.Application.Enum;
using ArtEva.Models.Enums;

namespace ArtEva.Application.Shops.Quiries
{
    public class ShopQueryCriteria
    {
        public int? OwnerUserId { get; set; }
        public ShopStatus? Status { get; set; }
        public decimal? MinRating { get; set; }
        public decimal? MaxRating { get; set; }
        public string? Name { get; set; }
        public ProductSortBy SortBy { get; set; } = ProductSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}
