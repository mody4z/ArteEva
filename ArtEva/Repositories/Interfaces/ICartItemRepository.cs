using ArteEva.Models;
using ArtEva.DTOs.Order;

namespace ArteEva.Repositories
{
   
    public interface ICartItemRepository : IRepository<CartItem>
    {
        IQueryable<CreateOrderFromCartItemDto> GetOrderInfoForCartItem(int cartItemId);

        IQueryable<CartItem> GetActiveItemsInCartQuery(int cartId);

        /// <summary>
        /// Gets queryable for all user's active cart items.
        /// </summary>
        IQueryable<CartItem> GetActiveItemsByUserQuery(int userId);

        /// <summary>
        /// Gets tracked item for update operations.
        /// </summary>
        Task<CartItem?> GetTrackedItemByCartAndProductAsync(int cartId, int productId);

        /// <summary>
        /// Gets all tracked active items in a cart.
        /// </summary>
        Task<List<CartItem>> GetTrackedActiveItemsInCartAsync(int cartId);

        /// <summary>
        /// Gets tracked item regardless of IsDeleted or IsConvertedToOrder status.
        /// </summary>
        Task<CartItem?> GetTrackedItemByCartAndProductIncludingDeletedAsync(int cartId, int productId);
    }
}
