using ArteEva.Models;
using ArtEva.Application.Enum;
using ArtEva.Application.Products.Quiries;
using System.Linq.Expressions;

namespace ArtEva.Application.Products.Specifications
{
    public class ProductQuerySpecification: BaseSpecification<Product>
    {
        public ProductQuerySpecification(ProductQueryCriteria criteria)
        {
            Criteria = p =>
                (!criteria.ShopId.HasValue || p.ShopId == criteria.ShopId.Value) &&
                (!criteria.Status.HasValue || p.Status == criteria.Status.Value) &&
                (!criteria.ApprovalStatus.HasValue || p.ApprovalStatus == criteria.ApprovalStatus.Value) &&
                (!criteria.IsPublished.HasValue || p.IsPublished == criteria.IsPublished.Value);
            ApplySorting(criteria);
        }

        private void ApplySorting(ProductQueryCriteria criteria)
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
