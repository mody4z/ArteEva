using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.Services;
using ArtEva.DTOs.CartDTOs;
using ArtEva.DTOs.CartItem;
using ArtEva.Services.Interfaces;
using ArtEva.Repositories.Interfaces;
namespace ArtEva.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
         private readonly CartDomainService _domain;
        private readonly IProductService _productService;
         public CartService(
         CartDomainService domain,
        IProductService productService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
             _domain = domain;
            _productService = productService;
         }
        public async Task<CartResponseDto> GetCartAsync(int userId)
        {


            var cart = await _unitOfWork.CartRepository.GetOrCreateTrackedCartAsync(userId);

            return Map(cart);
        }

        public async Task<CartItemDto> AddItemAsync(int userId, int productId, int quantity)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (!product.IsPublished)
                throw new Exception("Product not available");

            var cart = await _unitOfWork.CartRepository.GetOrCreateTrackedCartAsync(userId);

                    var item = _domain.AddItem(
                     cart,
                      productId,
                      product.Title,   // ✅ هنا
                      product.Price,
                          quantity
                                );

             if (item.Id == 0)
            {
                await _unitOfWork.CartItemRepository.AddAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();

            return MapItem(item);
        }

        public async Task<CartItemDto> UpdateItemAsync(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            var cart = await _unitOfWork.CartRepository.GetOrCreateTrackedCartAsync(userId);

            var item = await _unitOfWork.CartItemRepository
                .GetTrackedItemByCartAndProductAsync(cart.Id, productId);

            if (item == null)
                throw new Exception("Item not found in cart");

            item.Quantity = quantity;
            item.TotalPrice = item.UnitPrice * quantity;
            item.UpdatedAt = DateTime.UtcNow;


           await _unitOfWork.CartItemRepository.UpdateAsync(item);

            await _unitOfWork.SaveChangesAsync();

            return MapItem(item);
        }

        public async Task<CartItemDto> RemoveItemAsync(int userId, int productId)
        {
            var cart = await _unitOfWork.CartRepository.GetOrCreateTrackedCartAsync(userId);

            var item = _domain.RemoveItem(cart, productId);

            _unitOfWork.CartItemRepository.Remove(item);

            await _unitOfWork.SaveChangesAsync();

            var refreshed = await _unitOfWork.CartRepository.GetOrCreateTrackedCartAsync(userId);
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
