using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.CartDTOs;
using ArtEva.DTOs.CartItem;
using ArtEva.Services.Interfaces;
using System.Linq.Expressions;

namespace ArtEva.Services.Implementation
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository cartItemRepository;
        private readonly IProductRepository productRepository;

        public CartItemService(ICartItemRepository _cartItemRepository, IProductRepository _productRepository) {
        cartItemRepository = _cartItemRepository;
        productRepository = _productRepository;
        }
        public async Task<CartItemDto?> AddCartItem(AddCartItemDTO addCartItemDTO )
        { 
            // Fetch product details to populate required fields
            var product = await productRepository.GetByIdAsync(addCartItemDTO.ProductID);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {addCartItemDTO.ProductID} not found.");
            }

            CartItem cartItem = new CartItem()
            {
                CartId = addCartItemDTO.CartID,
                ProductId = addCartItemDTO.ProductID,
                Quantity = addCartItemDTO.Quantity,
                ProductName = product.Title,
                UnitPrice = product.Price,
                price = product.Price,
                UserId = addCartItemDTO.UserID
            };
            await cartItemRepository.AddAsync(cartItem);
            await cartItemRepository.SaveChanges();
         return new CartItemDto
            {
                Id = cartItem.Id,
                CartId = cartItem.CartId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                ProductName = cartItem.ProductName,
                Price = cartItem.price,
                Subtotal = cartItem.Quantity * cartItem.price
            };
        }
        
        public async Task<bool> DeleteCartItem(DeleteCartItemDTO deleteCartItemDTO)
        {
            // Find the cart item in the database
            var cartItem = await cartItemRepository.FirstOrDefaultAsync(ci => 
                ci.CartId == deleteCartItemDTO.CartID && 
                ci.ProductId == deleteCartItemDTO.ProductID && 
                !ci.IsDeleted);
            
            if (cartItem == null)
            {
                throw new InvalidOperationException($"Cart item with ProductId {deleteCartItemDTO.ProductID} not found in cart.");
            }

            // Soft delete the cart item
            cartItem.IsDeleted = true;
            cartItem.DeletedAt = DateTime.UtcNow;
            await cartItemRepository.UpdateAsync(cartItem);
            await cartItemRepository.SaveChanges();

           return true;

        }

        public async Task<IEnumerable< CartItemDto?>> GetALlCartitemInCart(int CartID)
        {
            //IQueryable<T> FindAsync(Expression<Func<T, bool>> predicate);
         IEnumerable<CartItem> cartItems=   cartItemRepository.FindAsync(c => c.CartId == CartID && c.IsDeleted == false).ToList();
            return cartItems.Select(c => new CartItemDto
            {
                Id = c.Id,
                CartId = c.CartId,
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Quantity = c.Quantity,
                Price = c.price,
                Subtotal = c.Quantity * c.price
            }).ToList();

        }

        public async Task<CartItemDto?> UpdateCartItem(UpdateCartitemDTO UpdateCartITemDTO)
        {
             

            CartItem cartItem = await cartItemRepository.GetByIdAsync(UpdateCartITemDTO.ID);
            cartItem.Quantity = UpdateCartITemDTO.Quantity;
           await cartItemRepository.SaveChanges();

          return new CartItemDto
            {
                Id = cartItem.Id,
                CartId = cartItem.CartId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                ProductName = cartItem.ProductName,
                Price = cartItem.price,
                Subtotal = cartItem.Quantity * cartItem.price
            };

        }
    }
}
