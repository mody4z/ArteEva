using ArteEva.Models;
using ArtEva.Application.Enum;
using ArtEva.Application.Products.Specifications;
using ArtEva.Models.Enums;

namespace ArtEva.Application.Products.Quiries
{
    public class PublicProductQueryCriteria
    {
        public int? ShopId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? Search { get; set; }

        public ProductSortBy SortBy { get; set; } = ProductSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}
