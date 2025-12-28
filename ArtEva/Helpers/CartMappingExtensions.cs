using System.Linq;
using System.Linq.Expressions;
using ArteEva.Models;
using ArtEva.DTOs.CartItem;

namespace ArtEva.Helpers
{
    /// <summary>
    /// Mapping extensions for CartItem entity.
    /// Uses Expression trees for SQL-level projection (performance optimization).
    /// </summary>
    public static class CartMappingExtensions
    {
        /// <summary>
        /// Projects IQueryable<CartItem> to IQueryable<CartItemDto>.
        /// Executes on database - only selected columns are fetched.
        /// </summary>
        public static IQueryable<CartItemDto> ProjectToCartItemDto(this IQueryable<CartItem> query)
        {
            return query.Select(ToCartItemDtoExpression);
        }

        /// <summary>
        /// Expression for SQL translation.
        /// EF Core converts this to SELECT Id, CartId, ProductId, etc.
        /// Calculates Subtotal using UnitPrice * Quantity (no TotalPrice column needed).
        /// </summary>
        private static Expression<Func<CartItem, CartItemDto>> ToCartItemDtoExpression =>
            item => new CartItemDto
            {
                Id = item.Id,
                CartId = item.CartId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.UnitPrice,
                Subtotal = item.UnitPrice * item.Quantity // ✅ Calculated in SQL
            };

        /// <summary>
        /// In-memory mapping for already-loaded entities.
        /// Use this after ToListAsync() when entity is already in memory.
        /// </summary>
        public static CartItemDto ToCartItemDto(this CartItem item)
        {
            return new CartItemDto
            {
                Id = item.Id,
                CartId = item.CartId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.UnitPrice,
                Subtotal = item.UnitPrice * item.Quantity // ✅ Calculated in memory
            };
        }
    }
}
