using ArteEva.Models;
using ArtEva.DTOs.CartDTOs;

namespace ArtEva.Services.Interfaces
{
    public interface ICartService
    {
        //IQueryable<Cart> QueryByUser(int userId);

        Task<CartResponseDto> GetOrCreateCartAsync(int userId);
        Task<CartResponseDto> AddItemToCartAsync(int userId, AddCartItemRequest request);
        Task<CartResponseDto> UpdateCartItemQuantityAsync(int userId, int productId, int quantity);
        Task<CartResponseDto> RemoveCartItemAsync(int userId, int productId);

        Task ClearCartAsync(int userId);
        Task<object> GetCartSummaryAsync(int userId);
          
        Task<Cart?> GetCartByUserIdAsync(int userId);
        
        Task<Cart?> GetCartWithItemsAsync(int userId);

     }
}
