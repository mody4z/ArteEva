using ArteEva.Models;
using ArtEva.DTOs.CartDTOs;
using ArtEva.DTOs.CartItem;

namespace ArtEva.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(int userId);
        Task<CartItemDto> AddItemAsync(int userId, int productId, int quantity);
        Task<CartItemDto> UpdateItemAsync(int userId, int productId, int quantity);
        Task<CartItemDto> RemoveItemAsync(int userId, int productId);

    }
}
