//using ArteEva.Models;
//using ArteEva.Repositories;
//using ArtEva.DTOs.CartDTOs;
//using ArtEva.Services.Interfaces;

//namespace ArtEva.Services.Implementation
//{
//    public class CartService : ICartService
//    {
//        private readonly ICartRepository _cartRepository;
//        private readonly ICartItemRepository _cartItemRepository;
//        private readonly IProductRepository _productRepository;

//        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IProductRepository productRepository)
//        {
//            _cartRepository = cartRepository;
//            _cartItemRepository = cartItemRepository;
//            _productRepository = productRepository;
//        }

//        public async Task<CartResponseDto> GetOrCreateCartAsync(int userId)
//        {
//            var cart = await _cartRepository.GetCartWithItemsAsync(userId);

//            // If cart doesn't exist, create it
//            if (cart == null)
//            {
//                cart = new Cart
//                {
//                    UserId = userId,
//                    CartItems = new List<CartItem>()
//                };
//                await _cartRepository.AddAsync(cart);
//                await _cartRepository.SaveChanges(); // Save to generate CartId

//                // Reload to ensure all navigation properties (like User) are handled
//                cart = await _cartRepository.GetCartWithItemsAsync(userId);
//            }

//            return MapToResponseDto(cart);
//        }

//        public async Task<CartResponseDto> AddItemToCartAsync(int userId, AddCartItemRequest request)
//        {
//            if (request == null) throw new ArgumentNullException(nameof(request));
//            if (request.Quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");

//            // 1. Get the cart (GetOrCreate handles the creation logic)
//            var cartResponse = await GetOrCreateCartAsync(userId);
//            var cart = await _cartRepository.GetCartWithItemsAsync(userId);

//            // 2. Check if product already exists in cart
//            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == request.ProductId && !i.IsDeleted);

//            if (existingItem != null)
//            {
//                existingItem.Quantity += request.Quantity;
//            }
//            else
//            {
//                // 3. Fetch product for details
//                var product = await _productRepository.GetByIdAsync(request.ProductId);
//                if (product == null) throw new InvalidOperationException($"Product {request.ProductId} not found.");

//                var newItem = new CartItem
//                {
//                    CartId = cart.Id,
//                    UserId = userId,
//                    ProductId = request.ProductId,
//                    Quantity = request.Quantity,
//                    UnitPrice = product.Price,
//                    price = product.Price,
//                    ProductName = product.Title
//                };
//                await _cartItemRepository.AddAsync(newItem);
//            }

//            // 4. SINGLE SAVE: This pushes all changes (updates or adds) to the DB
//            await _cartRepository.SaveChanges();

//            // 5. Return updated state
//            return await GetOrCreateCartAsync(userId);
//        }

//        public async Task<CartResponseDto> UpdateCartItemQuantityAsync(int userId, int productId, int quantity)
//        {
//            var cart = await _cartRepository.GetCartWithItemsAsync(userId);
//            if (cart == null) throw new InvalidOperationException("Cart not found.");

//            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId && !i.IsDeleted);
//            if (item == null) throw new InvalidOperationException("Item not found in cart.");

//            if (quantity <= 0)
//            {
//                await _cartItemRepository.Delete(item.Id);
//            }
//            else
//            {
//                item.Quantity = quantity;
//            }

//            await _cartRepository.SaveChanges();
//            return await GetOrCreateCartAsync(userId);
//        }

//        public async Task<CartResponseDto> RemoveCartItemAsync(int userId, int productId)
//        {
//            var cart = await _cartRepository.GetCartWithItemsAsync(userId);
//            if (cart == null) throw new InvalidOperationException("Cart not found.");

//            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId && !i.IsDeleted);
//            if (item != null)
//            {
//                await _cartItemRepository.Delete(item.Id);
//                await _cartRepository.SaveChanges();
//            }

//            return await GetOrCreateCartAsync(userId);
//        }

//        public async Task ClearCartAsync(int userId)
//        {
//            var cart = await _cartRepository.GetCartWithItemsAsync(userId);
//            if (cart == null) return;

//            foreach (var item in cart.CartItems.ToList())
//            {
//                await _cartItemRepository.Delete(item.Id);
//            }
//            await _cartRepository.SaveChanges();
//        }

//        public async Task<object> GetCartSummaryAsync(int userId)
//        {
//            var cart = await _cartRepository.GetCartWithItemsAsync(userId);
//            if (cart == null) return new { itemCount = 0, totalAmount = 0m };

//            return new
//            {
//                itemCount = cart.CartItems.Count(i => !i.IsDeleted),
//                totalAmount = cart.CartItems.Where(i => !i.IsDeleted).Sum(i => i.price * i.Quantity)
//            };
//        }

//        // Helper to keep code DRY
//        private CartResponseDto MapToResponseDto(Cart cart)
//        {
//            var items = cart.CartItems.Where(i => !i.IsDeleted).Select(item => new CartItemDto
//            {
//                Id = item.Id,
//                CartId = cart.Id,
//                ProductId = item.ProductId,
//                ProductName = item.ProductName,
//                Quantity = item.Quantity,
//                Price = item.price,
//                Subtotal = item.Quantity * item.price
//            }).ToList();

//            return new CartResponseDto
//            {
//                CartId = cart.Id,
//                UserId = cart.UserId,
//                UserName = cart.User?.UserName ?? "User",
//                Items = items,
//                TotalAmount = items.Sum(i => i.Subtotal),
//                ItemCount = items.Count
//            };
//        }
//    }
//}