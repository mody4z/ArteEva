using ArtEva.DTOs.CartDTOs;
using ArtEva.DTOs.CartItem;

namespace ArtEva.Services.Interfaces
{
    public interface ICartItemService
    {
        public Task<IEnumerable<CartItemDto?>> GetALlCartitemInCart(int CartID);
        public Task<CartItemDto?> AddCartItem(AddCartItemDTO addCartItemDTO);
        public   Task<CartItemDto?> UpdateCartItem(UpdateCartitemDTO UpdateCartITemDTO);
        public Task<bool> DeleteCartItem(DeleteCartItemDTO deleteCartItemDTO);


    }
}
