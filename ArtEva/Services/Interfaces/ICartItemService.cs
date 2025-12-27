using ArteEva.Models;
using ArtEva.DTOs.CartDTOs;
using ArtEva.DTOs.CartItem;

namespace ArtEva.Services.Interfaces
{
    /// <summary>
    /// Service contract for CartItem business operations.
    /// </summary>
    public interface ICartItemService
    {
        /// <summary>
        /// Adds new item or increments quantity if exists.
        /// </summary>
        Task AddOrIncrementItemAsync(int cartId, int userId, int productId, int quantity);

        /// <summary>
        /// Updates item quantity (replaces current value).
        /// </summary>
        Task UpdateItemQuantityAsync(int cartId, int productId, int newQuantity);

        /// <summary>
        /// Soft-deletes an item from cart.
        /// </summary>
        Task RemoveItemAsync(int cartId, int productId);

        /// <summary>
        /// Soft-deletes all items in a cart.
        /// </summary>
        Task ClearAllItemsInCartAsync(int cartId);

        /// <summary>
        /// Gets cart summary with SQL aggregation.
        /// </summary>
        Task<CartSummaryDto> GetCartSummaryAsync(int cartId);

        /// <summary>
        /// Gets items query for deferred execution.
        /// </summary>
        //IQueryable<CartItemDto> GetCartItemsQuery(int cartId);

        /// <summary>
        /// Gets materialized list of cart items.
        /// </summary>
        Task<List<CartItemDto>> GetCartItemsAsync(int cartId);
    }
}
