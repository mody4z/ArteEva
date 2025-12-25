using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.CartDTOs;
using ArtEva.DTOs.CartItem;
using ArtEva.Services.Interfaces;
namespace ArtEva.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository cartRepository;
        private readonly ICartItemService cartItemService;
 
        public CartService(ICartRepository _cartRepository, ICartItemService _cartItemService)
        {
            cartRepository = _cartRepository;
            cartItemService = _cartItemService;

         }

        public async Task<CartResponseDto> GetOrCreateCartAsync(int userId)
        {
            var cart = await cartRepository.FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
            CartResponseDto cartDto;
            if (cart != null)
            {
                var items =await cartItemService.GetALlCartitemInCart(cart.Id);

                cartDto = new CartResponseDto
                {
                    CartId = cart.Id,
                    UserId = cart.UserId,
                    UserName = cart.User?.UserName ?? "",
                    Items = items.Where(i => i != null).Select(i =>
                    {
                        i.CartId = cart.Id;
                        return i;
                    }).ToList(),
                    TotalAmount = items.Sum(i => i.Subtotal),
                    ItemCount = items.Count()
                };
                return cartDto;
            }

            cart = new Cart
            {
                UserId = userId,
                CartItems = new List<CartItem>()
            };
            
            await cartRepository.AddAsync(cart);
            await cartRepository.SaveChanges();
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
        public async Task<CartResponseDto> AddItemToCartAsync(int userId, AddCartItemRequest request)
        {
            // 1. Get or create the cart first and wait for it to be fully ready
            var cartDto = await GetOrCreateCartAsync(userId).ConfigureAwait(false);

            // 2. Prepare the item DTO
            AddCartItemDTO addCartItemDto = new AddCartItemDTO()
            {
                ProductID = request.ProductId,
                Quantity = request.Quantity,
                CartID = cartDto.CartId,
                UserID = userId
            };

            // 3. Add the item and wait for completion
            await cartItemService.AddCartItem(addCartItemDto).ConfigureAwait(false);

            // 4. Refresh and return the final state - ensure previous operation completed
            return await GetOrCreateCartAsync(userId).ConfigureAwait(false);
        }

        //public async Task<Cart?> GetCartByUserIdAsync(int userId)
        //{
        //    return 
        //}

        //public Task<Cart?> GetCartWithItemsAsync(int userId)
        //{
        //    return cartRepository.GetCartWithItemsAsync(userId);
        //}

        //public async Task<CartResponseDto> AddItemToCartAsync(int userId, AddCartItemRequest request)
        //{
        //    if (request == null) throw new ArgumentNullException(nameof(request));
        //    if (request.Quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");

        //    // 1. Get or create cart
        //    var cart = await cartRepository.GetOrCreateCartAsync(userId);

        //    if (cart == null)
        //    {
        //        // Create new cart if it doesn't exist
        //        cart = new Cart
        //        {
        //            UserId = userId,
        //            CartItems = new List<CartItem>()
        //        };
        //        await cartRepository.AddAsync(cart);

        //      }

        //    // 2. Check if product already exists in cart
        //    var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == request.ProductId && !i.IsDeleted);

        //    if (existingItem != null)
        //    {
        //        // Update existing item quantity (tracked by EF, no need to call Update)
        //        existingItem.Quantity += request.Quantity;
        //    }
        //    else
        //    {
        //        // 3. Fetch product for details
        //        var product = await _productRepository.GetByIdAsync(request.ProductId);
        //        if (product == null) throw new InvalidOperationException($"Product {request.ProductId} not found.");

        //        var newItem = new CartItem
        //        {
        //            CartId = cart.Id,
        //            UserId = userId,
        //            ProductId = request.ProductId,
        //            Quantity = request.Quantity,
        //            UnitPrice = product.Price,
        //            price = product.Price,
        //            ProductName = product.Title
        //        };
        //        await _cartItemRepository.AddAsync(newItem);
        //    }

        //    // 4. Save changes
        //    await _cartRepository.SaveChanges();

        //    // 5. Return updated state
        //    return await GetOrCreateCartAsync(userId);
        //}




        //public async Task<CartResponseDto> UpdateCartItemQuantityAsync(int userId, int productId, int quantity)
        //{
        //    if (quantity <= 0)
        //        throw new InvalidOperationException("Quantity must be greater than zero.");

        //    var cart = await GetCartWithItemsAsync(userId);
        //    if (cart == null)
        //        throw new InvalidOperationException("Cart not found.");

        //    await UpdateItemQuantityAsync(cart, productId, quantity);
        //    await _cartRepository.SaveChanges();
        //    await _cartItemRepository.SaveChanges();
        //    return await GetOrCreateCartAsync(userId);
        //}

        public async Task<CartResponseDto> RemoveCartItemAsync(RemoveItemDTO removeItemDTO)
        {
            var cart = await GetOrCreateCartAsync(removeItemDTO.userId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found.");
            DeleteCartItemDTO deleteCartItemDTO = new DeleteCartItemDTO()
            {
                CartID = cart.CartId,
                ProductID = removeItemDTO.productId
            };
            await cartItemService.DeleteCartItem(deleteCartItemDTO);
            cart = await GetOrCreateCartAsync(removeItemDTO.userId);
            return cart;
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found.");

            await ClearCartAsync(cart);
            await cartRepository.SaveChanges();
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

      
      

        private async Task ClearCartAsync(Cart cart)
        {
             cart.CartItems.Clear();
        }

        private bool IsCartEmpty(Cart cart)
        {
             return !cart.CartItems.Any();
        }

        private decimal CalculateCartTotal(Cart cart)
        {
 
            return cart.CartItems.Sum(i =>
                i.UnitPrice * i.Quantity);
        }

       

       

        public Task<CartResponseDto> UpdateCartItemQuantityAsync(int userId, int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Cart?> GetCartWithItemsAsync(int userId)
        {
            return cartRepository.GetOrCreateCartAsync(userId);
        }

        public async Task<CartResponseDto> RemoveCartItemAsync(int userId, int productId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found.");
            
            DeleteCartItemDTO deleteCartItemDTO = new DeleteCartItemDTO()
            {
                CartID = cart.CartId,
                ProductID = productId
            };
            
            await cartItemService.DeleteCartItem(deleteCartItemDTO);
            
            // Refresh and return the updated cart
            return await GetOrCreateCartAsync(userId);
        }
    }
}
