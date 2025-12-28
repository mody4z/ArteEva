using ArteEva.Models;
using ArtEva.Application.Enum;
using ArtEva.Application.Orders.Quiries;

namespace ArtEva.Application.Products.Specifications
{
    public class OrderQuerySpecification : BaseSpecification<Order>
    {
        public OrderQuerySpecification(OrderQueryCriteria criteria)
        {
            Criteria = o =>
                (!criteria.SellerUserId.HasValue || o.Shop.OwnerUserId == criteria.SellerUserId) &&
                (!criteria.BuyerUserId.HasValue || o.UserId == criteria.BuyerUserId) &&
                (!criteria.ShopId.HasValue || o.ShopId == criteria.ShopId) &&
                (!criteria.Status.HasValue || o.Status == criteria.Status) &&
                (!criteria.FromDate.HasValue || o.CreatedAt >= criteria.FromDate) &&
                (!criteria.ToDate.HasValue || o.CreatedAt <= criteria.ToDate) &&
                (!criteria.MinTotal.HasValue || o.GrandTotal >= criteria.MinTotal) &&
                (!criteria.MaxTotal.HasValue || o.GrandTotal <= criteria.MaxTotal) &&
                (string.IsNullOrEmpty(criteria.OrderNumber) || o.OrderNumber.Contains(criteria.OrderNumber));

            ApplySorting(criteria);
        }

        private void ApplySorting(OrderQueryCriteria criteria)
        {
            switch (criteria.SortBy)
            {
                case OrderSortBy.GrandTotal:
                    ApplyTotalSorting(criteria.SortDirection);
                    break;

                case OrderSortBy.Status:
                    ApplyStatusSorting(criteria.SortDirection);
                    break;

                default:
                    ApplyCreatedAtSorting(criteria.SortDirection);
                    break;
            }
        }

        private void ApplyTotalSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = o => o.GrandTotal;
            else
                OrderByDescending = o => o.GrandTotal;
        }

        private void ApplyStatusSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = o => o.Status;
            else
                OrderByDescending = o => o.Status;
        }

        private void ApplyCreatedAtSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = o => o.CreatedAt;
            else
                OrderByDescending = o => o.CreatedAt;
        }
    }

}
