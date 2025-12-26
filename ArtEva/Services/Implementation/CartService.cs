using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.CartDTOs;
using ArtEva.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtEva.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }

        /* ===================== QUERY ===================== */

        public IQueryable<Cart> QueryByUser(int userId)
        {
            return _cartRepository.QueryByUser(userId);
        }

        /* ===================== READ ===================== */

        public async Task<CartResponseDto> GetOrCreateCartAsync(int userId)
        {
            var cart = await _cartRepository.GetOrCreateCartWithTrackingAsync(userId);

            var items = await _cartItemRepository
                .QueryByCart(cart.Id)
                .Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    CartId = ci.CartId,
                    ProductId = ci.ProductId,
                    ProductName = ci.ProductName,
                    Quantity = ci.Quantity,
                    Price = ci.UnitPrice,
                    Subtotal = ci.UnitPrice * ci.Quantity
                })
                .ToListAsync();

            return new CartResponseDto
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = items,
                ItemCount = items.Count,
                TotalAmount = items.Sum(i => i.Subtotal)
            };
        }

        /* ===================== ADD ===================== */

        public async Task<CartResponseDto> AddItemToCartAsync(int userId, AddCartItemRequest request)
        {
            if (request.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero");

            var cart = await _cartRepository.GetOrCreateCartWithTrackingAsync(userId);

            var existingItem =
                await _cartItemRepository.GetByCartAndProductAsync(cart.Id, request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
            }
            else
            {
                var product = await _productRepository.GetByIdAsync(request.ProductId)
                    ?? throw new InvalidOperationException("Product not found");

                var item = new CartItem
                {
                    CartId = cart.Id,
                    UserId = userId,
                    ProductId = product.Id,
                    ProductName = product.Title,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * request.Quantity
                };

                await _cartItemRepository.AddAsync(item);
            }

            await _cartItemRepository.SaveChanges();
            return await GetOrCreateCartAsync(userId);
        }

        /* ===================== UPDATE ===================== */

        public async Task<CartResponseDto> UpdateCartItemQuantityAsync(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero");

            var cart = await _cartRepository.GetOrCreateCartWithTrackingAsync(userId);

            var item = await _cartItemRepository
                .GetByCartAndProductAsync(cart.Id, productId)
                ?? throw new InvalidOperationException("Cart item not found");

            item.Quantity = quantity;
            item.TotalPrice = item.UnitPrice * quantity;

            await _cartItemRepository.SaveChanges();
            return await GetOrCreateCartAsync(userId);
        }

        /* ===================== REMOVE ===================== */

        public async Task<CartResponseDto> RemoveCartItemAsync(int userId, int productId)
        {
            var cart = await _cartRepository.GetOrCreateCartWithTrackingAsync(userId);

            var item = await _cartItemRepository
                .GetByCartAndProductAsync(cart.Id, productId)
                ?? throw new InvalidOperationException("Item not found");

            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;

            await _cartItemRepository.SaveChanges();
            return await GetOrCreateCartAsync(userId);
        }

        /* ===================== CLEAR ===================== */

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetOrCreateCartWithTrackingAsync(userId);
            var items = await _cartItemRepository.GetNotConvertedByCartAsync(cart.Id);

            foreach (var item in items)
            {
                item.IsDeleted = true;
                item.DeletedAt = DateTime.UtcNow;
            }

            await _cartItemRepository.SaveChanges();
        }

        /* ===================== SUMMARY ===================== */

        public async Task<object> GetCartSummaryAsync(int userId)
        {
            var cart = await _cartRepository.GetOrCreateCartWithTrackingAsync(userId);

            var summary = await _cartItemRepository.QueryByCart(cart.Id)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    itemCount = g.Count(),
                    totalAmount = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .FirstOrDefaultAsync();

            return summary ?? new { itemCount = 0, totalAmount = 0m };
        }



        /* ===================== INTERNAL HELPERS (Missing implementations) ===================== */

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            // Read-only fast query: يرجع الـ Cart لو موجود (بدون تحميل العناصر)
            return await _cartRepository
                .QueryByUser(userId)
                .FirstOrDefaultAsync();
        }

        public async Task<Cart?> GetCartWithItemsAsync(int userId)
        {
            // نحصل على الـ Cart مع تتبع (create إذا مش موجود) لأن بعض المسارات تحتاج tracked entity
            var cart = await _cartRepository.GetOrCreateCartWithTrackingAsync(userId);

            if (cart == null)
                return null;

            // load cart items (only non-deleted & non-converted) using repository query (no heavy Include)
            var items = await _cartItemRepository
                .QueryByCart(cart.Id)
                .ToListAsync();

            // assign loaded items to the tracked cart instance
            cart.CartItems = items;

            return cart;
        }

    }
}

 