// CartItemService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.CartDTOs;
using ArtEva.Services.Interfaces;
using ArtEva.Helpers;
using ArtEva.DTOs.CartItem;

namespace ArtEva.Services.Implementation
{
    /// <summary>
    /// CartItem business logic service.
    /// Handles all operations related to individual cart items.
    /// </summary>
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductService _productService;

        public CartItemService(
            ICartItemRepository cartItemRepository,
            IProductService productService)
        {
            _cartItemRepository = cartItemRepository;
            _productService = productService;
        }

        /* ==================== COMMANDS (Write Operations) ==================== */

        /// <summary>
        /// Adds a new item or increments quantity if item already exists.
        /// Creates a product snapshot (name, price) for historical accuracy.
        /// </summary>
        public async Task AddOrIncrementItemAsync(int cartId, int userId, int productId, int quantity)
        {
            ValidateQuantity(quantity);

            // Validate product through ProductService (includes business rules)
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found.");

            // First check if item exists regardless of soft-delete status to avoid unique constraint violations
            var existingItem = await _cartItemRepository
                .GetTrackedItemByCartAndProductIncludingDeletedAsync(cartId, productId);

            if (existingItem != null)
            {
                // Reactivate if it was soft-deleted or converted
                if (existingItem.IsDeleted || existingItem.IsConvertedToOrder)
                {
                    existingItem.IsDeleted = false;
                    existingItem.IsConvertedToOrder = false;
                    existingItem.Quantity = quantity;
                    existingItem.ProductName = product.Title; // Update snapshot
                    existingItem.UnitPrice = product.Price; // Update snapshot
                    existingItem.TotalPrice = product.Price * quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Item is active, just increment quantity
                    existingItem.Quantity += quantity;
                    existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                // Create new item with product snapshot
                var newItem = new CartItem
                {
                    CartId = cartId,
                    UserId = userId,
                    ProductId = product.Id,
                    ProductName = product.Title,
                    UnitPrice = product.Price,
                    Quantity = quantity,
                    TotalPrice = product.Price * quantity,
                    IsDeleted = false,
                    IsConvertedToOrder = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _cartItemRepository.AddAsync(newItem);
            }

            await _cartItemRepository.SaveChanges();
        }

        /// <summary>
        /// Sets item quantity to a specific value (replaces current quantity).
        /// </summary>
        public async Task UpdateItemQuantityAsync(int cartId, int productId, int newQuantity)
        {
            ValidateQuantity(newQuantity);

            var item = await _cartItemRepository
                .GetTrackedItemByCartAndProductAsync(cartId, productId)
                ?? throw new KeyNotFoundException("Item not found in cart.");

            ValidateItemNotConverted(item);

            item.Quantity = newQuantity;
            item.TotalPrice = item.UnitPrice * newQuantity;
            item.UpdatedAt = DateTime.UtcNow;

            await _cartItemRepository.SaveChanges();
        }

        /// <summary>
        /// Soft-deletes a specific item from the cart.
        /// </summary>
        public async Task RemoveItemAsync(int cartId, int productId)
        {
            var item = await _cartItemRepository
                .GetTrackedItemByCartAndProductIncludingDeletedAsync(cartId, productId);
            
            if (item == null)
                throw new KeyNotFoundException($"Item with ProductId {productId} not found in cart.");

            ValidateItemNotConverted(item);
            
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;

            await _cartItemRepository.SaveChanges();
        }

        /// <summary>
        /// Soft-deletes all items in a cart (batch operation).
        /// </summary>
        public async Task ClearAllItemsInCartAsync(int cartId)
        {
            var items = await _cartItemRepository.GetTrackedActiveItemsInCartAsync(cartId);

            foreach (var item in items)
            {
                item.IsDeleted = true;
                item.DeletedAt = DateTime.UtcNow;
            }

            await _cartItemRepository.SaveChanges();
        }

        /* ==================== QUERIES (Read Operations) ==================== */

        /// <summary>
        /// Returns IQueryable for cart items (deferred execution).
        /// Allows caller to add filters, sorting, paging before execution.
        /// </summary>
        //public IQueryable<CartItemDto> GetCartItemsQuery(int cartId)
        //{
        //    return _cartItemRepository
        //        .GetActiveItemsInCartQuery(cartId)
        //        .ProjectToCartItemDto();
        //}

        /// <summary>
        /// Returns materialized list of cart items.
        /// </summary>
        public async Task<List<CartItemDto>> GetCartItemsAsync(int cartId)
        {
            return await _cartItemRepository
                .GetActiveItemsInCartQuery(cartId)
                .Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    CartId = ci.CartId,
                    ProductId = ci.ProductId,
                    ProductName = ci.ProductName,
                    Quantity = ci.Quantity,
                    Price = ci.UnitPrice,
                    Subtotal = ci.TotalPrice
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets cart summary with database-level aggregation (efficient).
        /// </summary>
        public async Task<CartSummaryDto> GetCartSummaryAsync(int cartId)
        {
            var summary = await _cartItemRepository
                .GetActiveItemsInCartQuery(cartId)
                .GroupBy(x => x.CartId)
                .Select(g => new CartSummaryDto
                {
                    ItemCount = g.Count(),
                    TotalAmount = g.Sum(x => x.TotalPrice)
                })
                .FirstOrDefaultAsync();

            return summary ?? new CartSummaryDto { ItemCount = 0, TotalAmount = 0m };
        }

        // Private validation helpers
        private static void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            if (quantity > 1000)
                throw new ArgumentException("Quantity cannot exceed 1000.", nameof(quantity));
        }

        private static void ValidateItemNotConverted(CartItem item)
        {
            if (item.IsConvertedToOrder)
                throw new InvalidOperationException("Cannot modify item that has been converted to an order.");
        }
    }
}
