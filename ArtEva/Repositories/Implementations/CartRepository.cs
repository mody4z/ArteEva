using ArteEva.Data;
using ArteEva.Models;
using Microsoft.EntityFrameworkCore;

namespace ArteEva.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<Cart> QueryByUser(int userId)
        {
            return _context.Carts
                .AsNoTracking()
                .Where(c => c.UserId == userId && !c.IsDeleted);
        }

        public async Task<Cart?> GetOrCreateCartWithTrackingAsync(int userId)
        {
            var cart = await _context.Carts
                .AsTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Cart?> GetByIdWithTrackingAsync(int cartId)
        {
            return await _context.Carts
                .AsTracking()
                .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted);
        }
    }
}
