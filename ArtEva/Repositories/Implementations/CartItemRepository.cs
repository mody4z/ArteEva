using ArteEva.Data;
using ArteEva.Models;
using ArtEva.DTOs.Order;
using Microsoft.EntityFrameworkCore;

namespace ArteEva.Repositories.Implementations
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

        public IQueryable<CartItem> QueryByCart(int cartId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CartItem> QueryByUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
