using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArtEva.DTOs.Order;
using ArtEva.DTOs.Orders;
using ArtEva.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtEva.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize] 
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
       
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserIdFromClaims();
            var orders = await _orderService.CreateOrdersFromCartAsync(userId);
            // return created orders
            return Ok(orders);
        }

        // ===================== GET =====================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var actorUserId = GetUserIdFromClaims();
            var order = await _orderService.GetOrderByIdAsync(id, actorUserId);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersForBuyer()
        {
            var buyerId = GetUserIdFromClaims();
            var orders = await _orderService.GetOrdersForBuyerAsync(buyerId);
            return Ok(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersForSeller()
        {
            var sellerId = GetUserIdFromClaims();
            var orders = await _orderService.GetOrdersForSellerAsync(sellerId);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> GetOrdersByIds([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0) return BadRequest("ids required");
            var orders = await _orderService.GetOrdersByIdsAsync(ids);
            return Ok(orders);
        }

        // ===================== SELLER ACTIONS =====================
        /// <summary>Seller proposes execution duration (moves SellerPending -> BuyerPending)</summary>
        [HttpPost("{id:int}/propose")]
        public async Task<IActionResult> ProposeExecutionBySeller(int id, [FromBody] ProposeExecutionDto dto)
        {
            if (dto == null) return BadRequest("ExecutionDays required");
            var sellerUserId = GetUserIdFromClaims();
            var updated = await _orderService.ProposeExecutionBySellerAsync(id, sellerUserId, dto.ExecutionDays);
            return Ok(updated);
        }

      

        /// <summary>Seller marks finished and ready for buyer confirmation (CompletedBySeller)</summary>
        [HttpPost("{id:int}/ready-for-delivery")]
        public async Task<IActionResult> ReadyForDelivery(int id)
        {
            var sellerUserId = GetUserIdFromClaims();
            var updated = await _orderService.MarkOrderWaitingDeliveryAsync(id, sellerUserId);
            return Ok(updated);
        }

        // ===================== BUYER ACTIONS =====================
        /// <summary>Buyer accepts or rejects proposed execution (BuyerPending -> InProgress or back to SellerPending)</summary>
        [HttpPost("{id:int}/confirm-execution")]
        public async Task<IActionResult> ConfirmExecutionByBuyer(int id, [FromBody] ConfirmExecutionDto dto)
        {
            if (dto == null) return BadRequest("Accept flag required");
            var buyerUserId = GetUserIdFromClaims();
            var updated = await _orderService.ConfirmExecutionByBuyerAsync(id, buyerUserId, dto.Accept);
            return Ok(updated);
        }

        /// <summary>Buyer confirms delivery (CompletedBySeller -> Delivered)</summary>
        [HttpPost("{id:int}/confirm-delivery")]
        public async Task<IActionResult> ConfirmDelivery(int id)
        {
            var buyerUserId = GetUserIdFromClaims();
            var updated = await _order_service_confirm_delivery(id, buyerUserId);
            return Ok(updated);
        }


        // ===================== CANCEL =====================
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderDto dto)
        {
            var actorUserId = GetUserIdFromClaims();
            await _orderService.CancelOrderAsync(id, actorUserId, dto?.Reason ?? string.Empty);
            return NoContent();
        }


        /// <summary>Seller starts work (optional) - sets InProgress when allowed</summary>
        [HttpPost("{id:int}/start")]
        public async Task<IActionResult> StartOrder(int id)
        {
            var sellerUserId = GetUserIdFromClaims();
            var updated = await _orderService.MarkOrderInProgressAsync(id, sellerUserId);
            return Ok(updated);
        }
        private int GetUserIdFromClaims()
        {
            var sub = User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(sub) && int.TryParse(sub, out var id1)) return id1;

            var nameId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(nameId) && int.TryParse(nameId, out var id2)) return id2;

            var name = User.Identity?.Name;
            if (!string.IsNullOrEmpty(name) && int.TryParse(name, out var id3)) return id3;

            throw new InvalidOperationException("Cannot determine user id from token.");
        }

        // small wrapper to call service and handle possible exceptions nicely
        private async Task<object> _order_service_confirm_delivery(int orderId, int buyerUserId)
        {
            var updated = await _orderService.ConfirmDeliveryByBuyerAsync(orderId, buyerUserId);
            return updated;
        }



    }
}
