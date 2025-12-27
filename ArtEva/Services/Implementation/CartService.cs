using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ArteEva.Repositories;
using ArtEva.DTOs.CartDTOs;
using ArtEva.Services.Interfaces;
using ArtEva.DTOs.CartItem;

namespace ArtEva.Services.Implementation
{
    /// <summary>
    /// Cart orchestration service.
    /// Coordinates between Cart and CartItem operations at user level.
    /// </summary>
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemService _cartItemService;

        public CartService(
            ICartRepository cartRepository,
            ICartItemService cartItemService)
        {
            _cartRepository = cartRepository;
            _cartItemService = cartItemService;
        }
        public async Task<CartResponseDto> GetOrCreateUserCartAsync(int userId)
        {
            // Ensure cart exists
            var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);

            // Query CartItems directly to ensure fresh data from database
            var items = await _cartItemService.GetCartItemsAsync(cart.Id);

            return new CartResponseDto
            {
                CartId = cart.Id,
                UserId = userId,
                Items = items,
                ItemCount = items.Count,
                TotalAmount = items.Sum(i => i.Subtotal)
            };
        }

        public async Task<CartResponseDto> AddItemToUserCartAsync(int userId, int productId, int quantity)
        {
            // Get cart ID without tracking
            var cartId = await _cartRepository.GetCartsByUserQuery(userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
            
            if (cartId == 0)
            {
                var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);
                cartId = cart.Id;
            }

            // Delegate to CartItemService
            await _cartItemService.AddOrIncrementItemAsync(cartId, userId, productId, quantity);

            return await GetOrCreateUserCartAsync(userId);
        }

        public async Task<CartResponseDto> UpdateItemInUserCartAsync(int userId, int productId, int quantity)
        {
            // Get cart ID without tracking
            var cartId = await _cartRepository.GetCartsByUserQuery(userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
            
            if (cartId == 0)
            {
                var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);
                cartId = cart.Id;
            }

            // Delegate to CartItemService
            await _cartItemService.UpdateItemQuantityAsync(cartId, productId, quantity);

            return await GetOrCreateUserCartAsync(userId);
        }

        public async Task<CartResponseDto> RemoveItemFromUserCartAsync(int userId, int productId)
        {
            // Get cart ID without tracking to avoid change tracker interference
            var cartId = await _cartRepository.GetCartsByUserQuery(userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
            
            if (cartId == 0)
            {
                // Cart doesn't exist, create it first
                var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);
                cartId = cart.Id;
            }
            
            // Delegate to CartItemService
            await _cartItemService.RemoveItemAsync(cartId, productId);

            // Return fresh cart data
            return await GetOrCreateUserCartAsync(userId);
        }
        public async Task ClearUserCartAsync(int userId)
        {
            // Ensure cart exists first
            var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);

            // Delegate to CartItemService
            await _cartItemService.ClearAllItemsInCartAsync(cart.Id);
        }

        public async Task<CartSummaryDto> GetUserCartSummaryAsync(int userId)
        {
            // Ensure cart exists first
            var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);

            // Delegate to CartItemService
            return await _cartItemService.GetCartSummaryAsync(cart.Id);
        }

        // Helper method to build response
        private static CartResponseDto BuildCartResponse(int cartId, int userId, List<CartItemDto> items)
        {
            return new CartResponseDto
            {
                CartId = cartId,
                UserId = userId,
                Items = items,
                ItemCount = items.Count,
                TotalAmount = items.Sum(i => i.Subtotal)
            };
        }
    }
}

