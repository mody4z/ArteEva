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
        public   Task<CartItemDto?> UpdateCartItem(UpdateCartitemDTO UpdateCartITemDTO);
        public Task<bool> DeleteCartItem(DeleteCartItemDTO deleteCartItemDTO);


    }
}
