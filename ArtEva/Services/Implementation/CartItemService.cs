// CartItemService.cs
using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.CartDTOs;
using ArtEva.DTOs.CartItem;
using ArtEva.DTOs.Order;
using ArtEva.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtEva.Services.Implementation
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;

        public CartItemService(
            ICartItemRepository cartItemRepository,
            IProductRepository productRepository)
        {
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }
        public async Task<CreateOrderFromCartItemDto?> GetOrderInfoForCartItemAsync(int cartItemId)
        {
           var CreateOrder =   _cartItemRepository.GetOrderInfoForCartItem(cartItemId).FirstOrDefault();
            return CreateOrder;
        }

        public async Task MarkAsConvertedAsync(int cartItemId, int orderId)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);

            if (cartItem == null)
                throw new InvalidOperationException("Cart item not found");

            cartItem.IsConvertedToOrder = true;
            cartItem.OrderId = orderId;

            await _cartItemRepository.SaveChanges();
        }

        public async Task<IEnumerable<CartItemDto?>> GetALlCartitemInCart(int cartId)
        {
            var items = await _cartItemRepository
                .QueryByCart(cartId)
                .ToListAsync();

            return items.Select(MapToDto).Cast<CartItemDto?>().ToList();
        }

       
        public async Task<CartItemDto?> AddCartItem(AddCartItemDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");

            var product = await _productRepository.GetByIdAsync(dto.ProductID)
                ?? throw new InvalidOperationException($"Product with ID {dto.ProductID} not found.");

             var existing = await _cartItemRepository.GetByCartAndProductAsync(dto.CartID, dto.ProductID);

            if (existing != null)
            {
                if (existing.IsConvertedToOrder)
                    throw new InvalidOperationException("This cart item has already been converted to an order.");

                existing.Quantity += dto.Quantity;
                existing.TotalPrice = existing.UnitPrice * existing.Quantity;

                 await _cartItemRepository.SaveChanges();
                return MapToDto(existing);
            }

            // Create new cart item with snapshots
            var cartItem = new CartItem
            {
                CartId = dto.CartID,
                UserId = dto.UserID,
                ProductId = product.Id,
                ProductName = product.Title ?? string.Empty,
                UnitPrice = product.Price,
                Quantity = dto.Quantity,
                TotalPrice = product.Price * dto.Quantity,
                IsDeleted = false,
                IsConvertedToOrder = false
            };

            await _cartItemRepository.AddAsync(cartItem);
            await _cartItemRepository.SaveChanges();

            return MapToDto(cartItem);
        }

        /// <summary>
        /// تحديث كمية عنصر في الكارت
        /// </summary>
        public async Task<CartItemDto?> UpdateCartItem(UpdateCartitemDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");

            var cartItem = await _cartItemRepository.GetByIdAsync(dto.ID)
                ?? throw new InvalidOperationException("Cart item not found.");

            if (cartItem.IsDeleted)
                throw new InvalidOperationException("Cannot update a deleted cart item.");

            if (cartItem.IsConvertedToOrder)
                throw new InvalidOperationException("Cannot update an item already converted to an order.");

            cartItem.Quantity = dto.Quantity;
            cartItem.TotalPrice = cartItem.UnitPrice * dto.Quantity;

            await _cartItemRepository.SaveChanges();

            return MapToDto(cartItem);
        }

        /// <summary>
        /// حذف عنصر (soft-delete)
        /// </summary>
        public async Task<bool> DeleteCartItem(DeleteCartItemDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var cartItem = await _cartItemRepository.FirstOrDefaultAsync(ci =>
                ci.CartId == dto.CartID &&
                ci.ProductId == dto.ProductID &&
                !ci.IsDeleted);

            if (cartItem == null)
                throw new InvalidOperationException("Cart item not found.");

            if (cartItem.IsConvertedToOrder)
                throw new InvalidOperationException("Cannot delete an item already converted to an order.");

            cartItem.IsDeleted = true;
            cartItem.DeletedAt = DateTime.UtcNow;

            await _cartItemRepository.SaveChanges();
            return true;
        }

        /* ----------------- Helpers ----------------- */

        private static CartItemDto MapToDto(CartItem c)
        {
            return new CartItemDto
            {
                Id = c.Id,
                CartId = c.CartId,
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Quantity = c.Quantity,
                Price = c.UnitPrice,
                Subtotal = c.TotalPrice
            };
        }
    }
}
