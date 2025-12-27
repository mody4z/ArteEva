using ArteEva.Data;
using ArteEva.Models;
using Microsoft.EntityFrameworkCore;

namespace ArteEva.Repositories.Implementations
{
    /// <summary>
    /// Repository for Cart entity - Pure data access, no business logic.
    /// </summary>
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context) { }

        /// <summary>
        /// Returns queryable for carts belonging to a user.
        /// Uses AsNoTracking for read-only queries.
        /// </summary>
        public IQueryable<Cart> GetCartsByUserQuery(int userId)
        {
            return _context.Carts
                .AsNoTracking()
                .Where(c => c.UserId == userId && !c.IsDeleted);
        }

        /// <summary>
        /// Gets or creates a tracked cart for the user.
        /// Returns tracked entity for subsequent updates.
        /// Never returns null - always creates if doesn't exist.
        /// </summary>
        /// GetOrCreateCartWithTrackingAsync
        public async Task<Cart> GetOrCreateTrackedCartAsync(int userId)
        {
            // Try to find existing active cart - explicitly ignore User navigation to prevent eager loading
            var cart = await _context.Carts
                .IgnoreAutoIncludes()
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                // Create new cart
                cart = new Cart
                {
                    UserId = userId,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
                
                // Reload to ensure we have the database-generated Id
                await _context.Entry(cart).ReloadAsync();
            }

            return cart;
        }

        /// <summary>
        /// Gets tracked cart by ID for update operations.
        /// Returns null if cart doesn't exist or is deleted.
        /// </summary>
        public async Task<Cart?> GetTrackedCartByIdAsync(int cartId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted);
        }
    }
}
