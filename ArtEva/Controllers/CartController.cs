using ArteEva.Models;
using ArtEva.DTOs.CartDTOs;
using ArtEva.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtEva.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Get current user's cart with all items
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out var userId))
                    return Unauthorized("Invalid user ID");

                var cart = await _cartService.GetOrCreateCartAsync(userId);
                 
 

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the cart", error = ex.Message });
            }
        }

        /// <summary>
        /// Add item to cart
        /// </summary>
        [HttpPost("items")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddCartItemRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = Int32.Parse(userIdString);

            var cart = await _cartService.GetOrCreateCartAsync(userId);

            await _cartService.AddItemAsync(
                cart.CartId,
                request.ProductId,
                request.UnitPrice,
                request.Quantity
            );

            return Ok(new { message = "Item added to cart successfully" });
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        [HttpPut("items/{productId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(int productId, [FromBody] UpdateCartItemRequest request)
        {
            try
            {
                if (request == null || request.Quantity <= 0)
                {
                    return BadRequest("Invalid quantity");
                }

                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out var userId))
                    return Unauthorized("Invalid user ID");

                var cart = await _cartService.GetCartWithItemsAsync(userId);
                if (cart == null)
                {
                    return NotFound("Cart not found");
                }

                await _cartService.UpdateItemQuantityAsync(cart, productId, request.Quantity);
                await _cartService.SaveAsync();

                return Ok(new { message = "Cart item updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating cart item", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveCartItem(int productId)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out var userId))
                    return Unauthorized("Invalid user ID");

                var cart = await _cartService.GetCartWithItemsAsync(userId);
                if (cart == null)
                {
                    return NotFound("Cart not found");
                }

                await _cartService.RemoveItemAsync(cart, productId);
                await _cartService.SaveAsync();

                return Ok(new { message = "Item removed from cart successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing cart item", error = ex.Message });
            }
        }

        /// <summary>
        /// Clear all items from cart
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out var userId))
                    return Unauthorized("Invalid user ID");

                var cart = await _cartService.GetCartWithItemsAsync(userId);
                if (cart == null)
                {
                    return NotFound("Cart not found");
                }

                await _cartService.ClearCartAsync(cart);
                await _cartService.SaveAsync();

                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while clearing cart", error = ex.Message });
            }
        }

        /// <summary>
        /// Get cart summary (item count and total)
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetCartSummary()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdString, out var userId))
                    return Unauthorized("Invalid user ID");

                var cart = await _cartService.GetCartWithItemsAsync(userId);
                if (cart == null || _cartService.IsCartEmpty(cart))
                {
                    return Ok(new
                    {
                        itemCount = 0,
                        totalAmount = 0
                    });
                }

                var response = new
                {
                    itemCount = cart.CartItems.Count,
                    totalAmount = _cartService.CalculateCartTotal(cart)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving cart summary", error = ex.Message });
            }
        }
    }

     

}
