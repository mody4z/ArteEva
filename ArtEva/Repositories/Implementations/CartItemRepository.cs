using ArteEva.Data;
using ArteEva.Models;
using Microsoft.EntityFrameworkCore;

namespace ArteEva.Repositories.Implementations
{
    /// <summary>
    /// Repository for CartItem entity - Pure data access, no business logic.
    /// </summary>
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext context) : base(context) { }

        /// <summary>
        /// Returns queryable for active items in a specific cart.
        /// Active = not deleted AND not converted to order.
        /// Uses AsNoTracking for read-only queries.
        /// </summary>
        public IQueryable<CartItem> GetActiveItemsInCartQuery(int cartId)
        {
            return _context.CartItems
                .AsNoTracking()
                .Where(item =>
                    item.CartId == cartId &&
                    !item.IsDeleted &&
                    !item.IsConvertedToOrder);
        }

        /// <summary>
        /// Returns queryable for active items across all user's carts.
        /// </summary>
        public IQueryable<CartItem> GetActiveItemsByUserQuery(int userId)
        {
            return _context.CartItems
                .AsNoTracking()
                .Where(item =>
                    item.UserId == userId &&
                    !item.IsDeleted &&
                    !item.IsConvertedToOrder);
        }

        /// <summary>
        /// Gets a tracked item for update operations.
        /// Returns null if not found or if item is deleted/converted.
        /// </summary>
        public async Task<CartItem?> GetTrackedItemByCartAndProductAsync(int cartId, int productId)
        {
            // íÌÈ Ãä íßæä ÇáÊÊÈÚ (Tracking) ãÝÚá åäÇ áíÊãßä EF ãä ÑÄíÉ ÇáÊÛííÑ Ýí IsDeleted
            return await _context.CartItems
                .FirstOrDefaultAsync(item =>
                    item.CartId == cartId &&
                    item.ProductId == productId &&
                    !item.IsDeleted &&
                    !item.IsConvertedToOrder);
        }

        /// <summary>
        /// Gets all tracked active items in a cart for batch operations.
        /// </summary>
        public async Task<List<CartItem>> GetTrackedActiveItemsInCartAsync(int cartId)
        {
            return await _context.CartItems
                .Where(item =>
                    item.CartId == cartId &&
                    !item.IsDeleted &&
                    !item.IsConvertedToOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a tracked item regardless of IsDeleted or IsConvertedToOrder status.
        /// Used to reactivate soft-deleted items and avoid unique constraint violations.
        /// </summary>
        public async Task<CartItem?> GetTrackedItemByCartAndProductIncludingDeletedAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(item =>
                    item.CartId == cartId &&
                    item.ProductId == productId);
        }
    }
}
