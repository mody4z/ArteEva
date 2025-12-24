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
                var items = cart.CartItems.Select(item => new CartItemDto
                {
                    Id = item.Id,
                    CartId = cart.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.price,
                    Subtotal = item.Quantity * item.price
                }).ToList();

                cartDto = new CartResponseDto
                {
                    CartId = cart.Id,
                    UserId = cart.UserId,
                    UserName = cart.User?.UserName ?? "",
                    Items = items,
                    TotalAmount = items.Sum(i => i.Subtotal),
                    ItemCount = items.Count
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
                Items = new List<CartItemDto>(),
                TotalAmount = 0,
                ItemCount = 0
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

        public async Task<CartResponseDto> AddItemToCartAsync(int userId, AddCartItemRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var cart = await GetOrCreateCartAsync(userId);

            await AddItemAsync(
                cart.CartId,
                request.ProductId,
                request.UnitPrice,
                request.Quantity
            );

            return await GetOrCreateCartAsync(userId);
        }

        public async Task<CartResponseDto> UpdateCartItemQuantityAsync(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var cart = await GetCartWithItemsAsync(userId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found.");

            await UpdateItemQuantityAsync(cart, productId, quantity);
            await SaveAsync();

            return await GetOrCreateCartAsync(userId);
        }

        public async Task<CartResponseDto> RemoveCartItemAsync(int userId, int productId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found.");

            await RemoveItemAsync(cart, productId);
            await SaveAsync();

            return await GetOrCreateCartAsync(userId);
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found.");

            await ClearCartAsync(cart);
            await SaveAsync();
        }

        public async Task<object> GetCartSummaryAsync(int userId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            
            if (cart == null || IsCartEmpty(cart))
            {
                return new
                {
                    itemCount = 0,
                    totalAmount = 0m
                };
            }

            return new
            {
                itemCount = cart.CartItems.Count,
                totalAmount = CalculateCartTotal(cart)
            };
        }

        private async Task UpdateItemQuantityAsync(
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

        private async Task RemoveItemAsync(Cart cart, int productId)
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

        private async Task ClearCartAsync(Cart cart)
        {
            EnsureItemsLoaded(cart);
            cart.CartItems.Clear();
        }

        private bool IsCartEmpty(Cart cart)
        {
            EnsureItemsLoaded(cart);
            return !cart.CartItems.Any();
        }

        private decimal CalculateCartTotal(Cart cart)
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
