using ArteEva.Models;
using ArtEva.Application.Enum;
using ArtEva.Application.Products.Quiries;
using ArtEva.Application.Products.Specifications;
using ArtEva.Application.Shops.Quiries;
using System.Linq.Expressions;

namespace ArtEva.Application.Shops.Specifications
{
    public class ShopQuerySpecification : BaseSpecification<Shop>
    {

        public ShopQuerySpecification(ShopQueryCriteria criteria)
        {
            Criteria = s =>
                (!criteria.OwnerUserId.HasValue || s.OwnerUserId == criteria.OwnerUserId.Value) &&
                (!criteria.Status.HasValue || s.Status == criteria.Status.Value) &&
                (!criteria.MinRating.HasValue || s.RatingAverage >= criteria.MinRating.Value) &&
                (!criteria.MaxRating.HasValue || s.RatingAverage <= criteria.MaxRating.Value) &&
                (string.IsNullOrEmpty(criteria.Name) || s.Name.Contains(criteria.Name));
         ApplySorting(criteria);
        }
    

    private void ApplySorting(ShopQueryCriteria criteria)
        {
            switch (criteria.SortBy)
            {
                case ProductSortBy.Price:
                    ApplyNameSorting(criteria.SortDirection);
                    break;

                case ProductSortBy.Title:
                    ApplyRateSorting(criteria.SortDirection);
                    break;

                default:
                    ApplyCreatedAtSorting(criteria.SortDirection);
                    break;
            }
        }

        private void ApplyNameSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = s => s.Name;
            else
                OrderByDescending = p => p.Name;
        }

        private void ApplyRateSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = p => p.RatingAverage;
            else
                OrderByDescending = p => p.RatingAverage;
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
