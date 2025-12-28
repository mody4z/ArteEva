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
                .Where(c => c.UserId == userId && !c.IsDeleted);  // ✅ FIXED: UserId instead of Id
        }

        /// <summary>
        /// Gets or creates a tracked cart for the user.
        /// Returns tracked entity for subsequent updates.
        /// Never returns null - always creates if doesn't exist.
        /// </summary>
        public async Task<Cart> GetOrCreateTrackedCartAsync(int userId)  // ✅ FIXED: Return Cart
        {
            // Try to find existing active cart
            var cart = await _context.Carts
                .Include(c => c.CartItems.Where(ci => !ci.IsDeleted && !ci.IsConvertedToOrder))  // ✅ ADDED: Include CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);  // ✅ FIXED: UserId

            if (cart == null)
            {
                // Create new cart
                cart = new Cart
                {
                    UserId = userId,  // ✅ FIXED: Set UserId, not Id
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()  // ✅ ADDED: Initialize collection
                };

                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;  // ✅ FIXED: Return Cart entity
        }

        /// <summary>
        /// Gets tracked cart by ID for update operations.
        /// Returns null if cart doesn't exist or is deleted.
        /// </summary>
        public async Task<Cart?> GetTrackedCartByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems.Where(ci => !ci.IsDeleted && !ci.IsConvertedToOrder))  // ✅ ADDED
                .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted);
        }

        /// <summary>
        /// Saves changes to database.
        /// </summary>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
