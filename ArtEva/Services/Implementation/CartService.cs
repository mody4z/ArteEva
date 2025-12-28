using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.Services;
using ArtEva.DTOs.CartDTOs;
using ArtEva.DTOs.CartItem;
using ArtEva.Services.Interfaces;
namespace ArtEva.Services.Implementation
{
    public class CartService : ICartService
    {

        private readonly ICartRepository _cartRepository;
        private readonly CartDomainService _domain;
        private readonly IProductService _productService;
        private readonly ICartItemRepository _cartItemRepository;
        public CartService(
        ICartRepository cartRepository,
        CartDomainService domain,
        IProductService productService, ICartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _domain = domain;
            _productService = productService;
            _cartItemRepository = cartItemRepository;
        }
        public async Task<CartResponseDto> GetCartAsync(int userId)
        {


            var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);

            return Map(cart);
        }

        public async Task<CartItemDto> AddItemAsync(int userId, int productId, int quantity)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (!product.IsPublished)
                throw new Exception("Product not available");

            var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);

                    var item = _domain.AddItem(
                     cart,
                      productId,
                      product.Title,   // ✅ هنا
                      product.Price,
                          quantity
                                );

             if (item.Id == 0)
            {
                await _cartItemRepository.AddAsync(item);
            }

            await _cartRepository.SaveAsync();

            return MapItem(item);
        }

        public async Task<CartItemDto> UpdateItemAsync(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);

            var item = await _cartItemRepository
                .GetTrackedItemByCartAndProductAsync(cart.Id, productId);

            if (item == null)
                throw new Exception("Item not found in cart");

            item.Quantity = quantity;
            item.TotalPrice = item.UnitPrice * quantity;
            item.UpdatedAt = DateTime.UtcNow;


           await _cartItemRepository.UpdateAsync(item);

            await _cartRepository.SaveAsync();

            return MapItem(item);
        }

        public async Task<CartItemDto> RemoveItemAsync(int userId, int productId)
        {
            var cart = await _cartRepository.GetOrCreateTrackedCartAsync(userId);

            var item = _domain.RemoveItem(cart, productId);

            _cartItemRepository.Remove(item);

            await _cartRepository.SaveAsync();

            var refreshed = await _cartRepository.GetOrCreateTrackedCartAsync(userId);
            return MapItem(item);
        }

        private static CartItemDto MapItem(CartItem item)
        {
            return new CartItemDto
            {
                Id = item.Id,
                CartId = item.CartId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.UnitPrice,
                Subtotal = item.TotalPrice
            };
        }

        private static CartResponseDto Map(Cart cart)
        {
            return new CartResponseDto
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    CartId = i.CartId,
                    ProductName = i.ProductName,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.UnitPrice,
                    Subtotal = i.TotalPrice
                }).ToList(),
                ItemCount = cart.CartItems.Count,
                TotalAmount = cart.CartItems.Sum(i => i.TotalPrice)
            };
        }

    }
}
