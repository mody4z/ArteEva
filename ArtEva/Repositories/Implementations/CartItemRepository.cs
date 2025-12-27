using ArteEva.Data;
using ArteEva.Models;
using ArtEva.DTOs.Order;
using Microsoft.EntityFrameworkCore;

namespace ArteEva.Repositories
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<CreateOrderFromCartItemDto> GetOrderInfoForCartItem(int cartItemId)
        {
            return
                GetAllAsync()
                .Where(ci => ci.Id == cartItemId)
                .Select(ci => new CreateOrderFromCartItemDto
                {
                    CartItemId = ci.Id,
                    UserId = ci.Cart.UserId,

                    ProductId = ci.ProductId,
                    ShopId = ci.Product.ShopId,

                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    Subtotal = ci.TotalPrice,

                    ProductTitle = ci.ProductName,
                    ProductImage =  "", // ci.Product.ProductImages.Where(img=>img.IsPrimary==true).Select(img=>img.Url).FirstOrDefault(),
                    ExecutionDays = 1,
                    IsConvertedToOrder = ci.IsConvertedToOrder
                });
               
        }
         
        public IQueryable<CartItem> QueryByCart(int cartId)
        {
            return _context.CartItems
                .AsNoTracking()
                .Where(ci =>
                    ci.CartId == cartId &&
                    !ci.IsDeleted &&
                    !ci.IsConvertedToOrder);
        }

        public IQueryable<CartItem> QueryByUser(int userId)
        {
            return _context.CartItems
                .AsNoTracking()
                .Where(ci =>
                    ci.UserId == userId &&
                    !ci.IsDeleted &&
                    !ci.IsConvertedToOrder);
        }

        public async Task<CartItem?> GetByCartAndProductAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .AsTracking()
                .FirstOrDefaultAsync(ci =>
                    ci.CartId == cartId &&
                    ci.ProductId == productId &&
                    !ci.IsDeleted &&
                    !ci.IsConvertedToOrder);
        }

        public async Task<IEnumerable<CartItem>> GetNotConvertedByCartAsync(int cartId)
        {
            return await _context.CartItems
                .Where(ci =>
                    ci.CartId == cartId &&
                    !ci.IsDeleted &&
                    !ci.IsConvertedToOrder)
                .ToListAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<CartItem> items)
        {
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
