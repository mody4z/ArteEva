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
/*{
  "userName": "superadmin",
  "password": "Admin@123*"
}*/
public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's cart with all items
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();
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
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.AddItemToCartAsync(userId, request);
                return Ok(new { message = "Item added to cart successfully", cart });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding item to cart", error = ex.Message });
            }
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
                    return BadRequest(new { message = "Invalid quantity" });

                var userId = GetUserId();
                var cart = await _cartService.UpdateCartItemQuantityAsync(userId, productId, request.Quantity);
                return Ok(new { message = "Cart item updated successfully", cart });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
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
                var userId = GetUserId();
                var cart = await _cartService.RemoveCartItemAsync(userId, productId);
                return Ok(new { message = "Item removed from cart successfully", cart });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
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
                var userId = GetUserId();
                await _cartService.ClearCartAsync(userId);
                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
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
                var userId = GetUserId();
                var summary = await _cartService.GetCartSummaryAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving cart summary", error = ex.Message });
            }
        }

        private int GetUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID");
            return userId;
        }
    }

     

}
