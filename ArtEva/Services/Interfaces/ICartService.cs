using ArteEva.Models;
using ArtEva.DTOs.CartDTOs;

namespace ArtEva.Services.Interfaces
{
    /// <summary>
    /// Service contract for Cart orchestration.
    /// </summary>
    public interface ICartService
    {
        Task<CartResponseDto> GetOrCreateUserCartAsync(int userId);
        Task<CartResponseDto> AddItemToUserCartAsync(int userId, int productId, int quantity);
        Task<CartResponseDto> UpdateItemInUserCartAsync(int userId, int productId, int quantity);
        Task<CartResponseDto> RemoveItemFromUserCartAsync(int userId, int productId);
        Task ClearUserCartAsync(int userId);
        Task<CartSummaryDto> GetUserCartSummaryAsync(int userId);
    }
}
