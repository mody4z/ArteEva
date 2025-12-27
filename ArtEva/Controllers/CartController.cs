using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtEva.DTOs.CartDTOs;
using ArtEva.Services.Interfaces;
using System.Security.Claims;
using ArtEva.DTOs.CartItem;

namespace ArtEva.Controllers
{
    /// <summary>
    /// Shopping cart API endpoints.
    /// All endpoints require authentication.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Gets current user's cart with all items.
        /// </summary>
        /// <returns>Complete cart information</returns>
        [HttpGet]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.GetOrCreateUserCartAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets cart summary (item count and total amount).
        /// Useful for displaying in header/navigation.
        /// </summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(CartSummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartSummaryDto>> GetCartSummary()
        {
            try
            {
                var userId = GetCurrentUserId();
                var summary = await _cartService.GetUserCartSummaryAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Adds an item to cart or increments quantity if already exists.
        /// </summary>
        [HttpPost("items")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartResponseDto>> AddItem([FromBody] AddItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.AddItemToUserCartAsync(
                    userId,
                    request.ProductId,
                    request.Quantity);

                return Ok(cart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates item quantity in cart (replaces current value).
        /// </summary>
        [HttpPut("items")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartResponseDto>> UpdateItem([FromBody] UpdateItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.UpdateItemInUserCartAsync(
                    userId,
                    request.ProductId,
                    request.Quantity);

                return Ok(cart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Removes an item from cart.
        /// </summary>
        [HttpDelete("items/{productId}")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartResponseDto>> RemoveItem(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.RemoveItemFromUserCartAsync(userId, productId);
                return Ok(cart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Clears all items from cart.
        /// </summary>
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _cartService.ClearUserCartAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Helper to extract userId from JWT claims
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }
            return userId;
        }
    }
}
