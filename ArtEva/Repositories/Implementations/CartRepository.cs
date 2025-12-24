    using ArteEva.Data;
    using ArteEva.Models;
    using Microsoft.EntityFrameworkCore;

    namespace ArteEva.Repositories
    {
        public class CartRepository : Repository<Cart>, ICartRepository
        {
            public CartRepository(ApplicationDbContext context) : base(context)
            {
            }
            public async Task<Cart?> GetOrCreateCartAsync(int userId)
            {
                var cart = await _context.Carts
                    .AsTracking()
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c =>
                        c.UserId == userId &&
                        !c.IsDeleted);
                
                if (cart != null)
                {
                    // Separately load only non-deleted cart items
                    await _context.Entry(cart)
                        .Collection(c => c.CartItems)
                        .Query()
                        .Where(ci => !ci.IsDeleted)
                        .LoadAsync();
                }
                
                return cart;
            }
        }
    }
