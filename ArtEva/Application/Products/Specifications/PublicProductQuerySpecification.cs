using ArteEva.Models;
using ArtEva.Application.Enum;
using ArtEva.Application.Products.Quiries;
using ArtEva.Models.Enums;
using System.Linq.Expressions;

namespace ArtEva.Application.Products.Specifications
{
    public class PublicProductQuerySpecification: BaseSpecification<Product>
    {
        public PublicProductQuerySpecification(PublicProductQueryCriteria criteria)
        {
            Criteria = p =>
                p.Status == ProductStatus.Active &&
                p.ApprovalStatus == ProductApprovalStatus.Approved &&
                p.IsPublished &&

                (!criteria.ShopId.HasValue || p.ShopId == criteria.ShopId) &&
                (!criteria.CategoryId.HasValue || p.CategoryId == criteria.CategoryId) &&
                (!criteria.SubCategoryId.HasValue || p.SubCategoryId == criteria.SubCategoryId) &&
                (!criteria.MinPrice.HasValue || p.Price >= criteria.MinPrice) &&
                (!criteria.MaxPrice.HasValue || p.Price <= criteria.MaxPrice) &&
                (string.IsNullOrWhiteSpace(criteria.Search) ||
                    p.Title.Contains(criteria.Search));

            ApplySorting(criteria);
        }

        private void ApplySorting(PublicProductQueryCriteria criteria)
        {
            switch (criteria.SortBy)
            {
                case ProductSortBy.Price:
                    ApplyPriceSorting(criteria.SortDirection);
                    break;

                case ProductSortBy.Title:
                    ApplyTitleSorting(criteria.SortDirection);
                    break;

                default:
                    ApplyCreatedAtSorting(criteria.SortDirection);
                    break;
            }
        }

        private void ApplyPriceSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = p => p.Price;
            else
                OrderByDescending = p => p.Price;
        }

        private void ApplyTitleSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = p => p.Title;
            else
                OrderByDescending = p => p.Title;
        }

        private void ApplyCreatedAtSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = p => p.CreatedAt;
            else
                OrderByDescending = p => p.CreatedAt;
        }
    }
}
