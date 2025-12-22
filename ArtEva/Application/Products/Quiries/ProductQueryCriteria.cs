using ArtEva.Application.Enum;
using ArtEva.Models.Enums;

namespace ArtEva.Application.Products.Quiries
{
    public class ProductQueryCriteria
    {
        public int? ShopId { get; set; }
        public ProductStatus? Status { get; set; }
        public ProductApprovalStatus? ApprovalStatus { get; set; }
        public bool? IsPublished { get; set; }
        public ProductSortBy SortBy { get; set; } = ProductSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}
