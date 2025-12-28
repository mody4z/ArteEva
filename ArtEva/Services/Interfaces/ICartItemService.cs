using ArteEva.Models;
using ArtEva.DTOs.CartDTOs;
using ArtEva.DTOs.CartItem;
using ArtEva.DTOs.Order;

namespace ArtEva.Services.Interfaces
{

    public interface ICartItemService
    {
        public Task<CreateOrderFromCartItemDto?> GetOrderInfoForCartItemAsync(int cartItemId);
        public Task MarkAsConvertedAsync(int cartItemId, int orderId);

        public Task<IEnumerable<CartItemDto?>> GetALlCartitemInCart(int CartID);
        public Task<CartItemDto?> AddCartItem(AddCartItemDTO addCartItemDTO);
        public Task<CartItemDto?> UpdateCartItem(UpdateCartitemDTO UpdateCartITemDTO);
        public Task<bool> DeleteCartItem(DeleteCartItemDTO deleteCartItemDTO);

        Task AddOrIncrementItemAsync(int cartId, int userId, int productId, int quantity);

        Task UpdateItemQuantityAsync(int cartId, int productId, int newQuantity);

        Task RemoveItemAsync(int cartId, int productId);

        Task ClearAllItemsInCartAsync(int cartId);

        Task<CartSummaryDto> GetCartSummaryAsync(int cartId);

        Task<List<CartItemDto>> GetCartItemsAsync(int cartId);
    }
}
