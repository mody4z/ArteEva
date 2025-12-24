using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.CartDTOs;
using ArtEva.Services.Interfaces;
namespace ArtEva.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }

        public async Task<CartResponseDto> GetOrCreateCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(userId);
            CartResponseDto cartDto;
            if (cart != null)
            {
                cartDto = new CartResponseDto
                {
                    CartId = cart.Id,
                    UserId = cart.UserId,
                    UserName = cart.User?.UserName ?? "",
                    Items = cart.CartItems.Select(item => new CartItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.price
                    }).ToList()
                };
                return cartDto;
            }

            cart = new Cart
            {
                UserId = userId,
                CartItems = new List<CartItem>()
            };
            
            await _cartRepository.AddAsync(cart);
            await SaveAsync();
            
            cartDto = new CartResponseDto
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                UserName = cart.User?.UserName ?? "",
                Items = new List<CartItemDto>()
            };

            return cartDto;
        }

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _cartRepository.FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
        }

        public Task<Cart?> GetCartWithItemsAsync(int userId)
        {
            return _cartRepository.GetCartWithItemsAsync(userId);
        }

        public async Task AddItemAsync(
            int cartId,
            int productId,
            decimal unitPriceSnapshot,
            int quantity)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                throw new InvalidOperationException("Cart not found.");
            }
            EnsureItemsLoaded(cart);

            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var existing = cart.CartItems
                .FirstOrDefault(i => i.ProductId == productId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                return;
            }

            // Fetch product to get its title
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {productId} not found.");

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPriceSnapshot,
                price = unitPriceSnapshot,
                ProductName = product.Title
            };

            await _cartItemRepository.AddAsync(cartItem);
            await _cartItemRepository.SaveChanges();
            await _cartRepository.SaveChanges();
        }

        public async Task UpdateItemQuantityAsync(
            Cart cart,
            int productId,
            int quantity)
        {
            EnsureItemsLoaded(cart);

            var item = cart.CartItems
                .FirstOrDefault(i => i.ProductId == productId)
                ?? throw new InvalidOperationException("Item not found in cart.");

            if (quantity <= 0)
            {
                cart.CartItems.Remove(item);
                await _cartItemRepository.Delete(item.Id);
                return;
            }

            item.Quantity = quantity;
        }

        public async Task RemoveItemAsync(Cart cart, int productId)
        {
            EnsureItemsLoaded(cart);

            var item = cart.CartItems
                .FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                cart.CartItems.Remove(item);
                await _cartItemRepository.Delete(item.Id);
            }
        }

        public async Task ClearCartAsync(Cart cart)
        {
            EnsureItemsLoaded(cart);
            cart.CartItems.Clear();
        }

        public bool IsCartEmpty(Cart cart)
        {
            EnsureItemsLoaded(cart);
            return !cart.CartItems.Any();
        }

        public decimal CalculateCartTotal(Cart cart)
        {
            EnsureItemsLoaded(cart);

            return cart.CartItems.Sum(i =>
                i.UnitPrice * i.Quantity);
        }

        public async Task SaveAsync()
        {
            await _cartRepository.SaveChanges();
            await _cartItemRepository.SaveChanges();
        }

        private static void EnsureItemsLoaded(Cart cart)
        {
            if (cart.CartItems == null)
                throw new InvalidOperationException(
                    "CartItems not loaded. Use GetCartWithItemsAsync.");
        }
    }
}
