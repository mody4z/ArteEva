using ArteEva.Models;
using ArtEva.DTOs.CartDTOs;
using ArtEva.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtEva.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
     

        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok(await _cartService.GetCartAsync(GetUserId()));

        [HttpPost("add")]
        public async Task<IActionResult> Add(AddCartItemRequest dto)
            => Ok(await _cartService.AddItemAsync(GetUserId(), dto.ProductId, dto.Quantity));

        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateCartItemRequest dto)
            => Ok(await _cartService.UpdateItemAsync(GetUserId(), dto.ProductId, dto.Quantity));

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
            => Ok(await _cartService.RemoveItemAsync(GetUserId(), productId));
        
        
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.clear(GetUserId());
            return Ok();
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
