using ArtEva.Application.Orders.Quiries;
using ArtEva.DTOs.Order;
using ArtEva.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ArtEva.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize] 
    public class OrderController : ControllerBase
    {
        private readonly IOrderOrchestrator _orderOrchestrator;
        private readonly IOrderService _orderService;

        public OrderController (IOrderOrchestrator orderOrchestrator ,IOrderService orderService)
        {
            _orderOrchestrator = orderOrchestrator;
            _orderService = orderService;
        }
       
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(int cartItemId)
        {
            var userId = GetUserIdFromClaims();
            var orders = await _orderOrchestrator.CreateOrderFromCartItemAsync(cartItemId, userId);
            return Ok(orders);
        }

         #region Getting
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var actorUserId = GetUserIdFromClaims();
            var order = await _orderService.GetOrderByIdAsync(orderId, actorUserId);
            return Ok(order);
        }

        [HttpGet("buyer")]
        public async Task<IActionResult> GetOrdersForBuyer(
     [FromQuery] OrderQueryCriteria criteria,
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 20)
        {
            var buyerId = GetUserIdFromClaims();

            var result = await _orderService.GetOrdersForBuyerAsync(
                buyerId,
                criteria,
                pageNumber,
                pageSize);

            return Ok(result);
        }
        [HttpGet("seller")]
        public async Task<IActionResult> GetOrdersForSeller(
            [FromQuery] OrderQueryCriteria criteria,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var sellerId = GetUserIdFromClaims();

            var result = await _orderService.GetOrdersForSellerAsync(
                sellerId,
                criteria,
                pageNumber,
                pageSize);

            return Ok(result);
        }

        #endregion



        #region SELLER ACTIONS
        //// ===================== SELLER ACTIONS =====================
        ///// <summary>Seller proposes execution duration (moves SellerPending -> BuyerPending)</summary>
        [HttpPut("{orderId:int}")]
        public async Task<IActionResult> ProposeExecutionBySeller(int orderId, [FromBody] ProposeExecutionDto dto)
        {
            if (dto == null) return BadRequest("ExecutionDays required");
            var sellerUserId = GetUserIdFromClaims();
            var updated = await _orderOrchestrator.ProposeExecutionBySellerAsync(orderId, sellerUserId, dto.ExecutionDays);
            return Ok(updated);
        }



        ///// <summary>Seller marks finished and ready for buyer confirmation (CompletedBySeller)</summary>
        [HttpPut("{orderId:int}")]
        public async Task<IActionResult> ReadyForDelivery(int orderId)
        {
            var sellerUserId = GetUserIdFromClaims();
            var updated = await _orderOrchestrator.MarkOrderWaitingDeliveryAsync(orderId, sellerUserId);
            return Ok(updated);
        }

        //// ===================== BUYER ACTIONS =====================
        ///// <summary>Buyer accepts or rejects proposed execution (BuyerPending -> InProgress or back to SellerPending)</summary>
        [HttpPut("{orderId:int}")]
        public async Task<IActionResult> ConfirmExecutionByBuyer(int orderId, [FromBody] ConfirmExecutionDto dto)
        {
            if (dto == null) return BadRequest("Accept flag required");
            var buyerUserId = GetUserIdFromClaims();
            var updated = await _orderService.ConfirmExecutionByBuyerAsync(orderId, buyerUserId, dto.Accept);
            return Ok(updated);
        }

        ///// <summary>Buyer confirms delivery (CompletedBySeller -> Delivered)</summary>
        [HttpPut("{orderId:int}")]
        public async Task<IActionResult> ConfirmDelivery(int orderId)
        {
            var buyerUserId = GetUserIdFromClaims();
            var updated = await _orderService.ConfirmDeliveryByBuyerAsync(orderId, buyerUserId);
            return Ok(updated);
        }

        #endregion

        #region CANCEL
        //// ===================== CANCEL =====================
        [HttpPut("{orderId:int}")]
        public async Task<IActionResult> CancelOrder(int orderId, [FromBody] CancelOrderDto dto)
        {
            var actorUserId = GetUserIdFromClaims();
            await _orderOrchestrator.CancelOrderAsync(orderId, actorUserId, dto?.Reason ?? string.Empty);
            return NoContent();
        } 
        #endregion

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

     
    }
}
