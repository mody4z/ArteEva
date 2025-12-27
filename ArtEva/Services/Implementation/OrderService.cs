// OrderService.cs
using ArteEva.Data;
using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.Order;
using ArtEva.DTOs.Orders;
using ArtEva.Models.Enums;
using ArtEva.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtEva.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
 

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        #region Create Order

     
        public async Task<Order> CreateFromCartItemAsync(CreateOrderFromCartItemDto data)
        {
            var pricing = CalculatePricing(data.Subtotal);
            var orderNumber = GenerateOrderNumber();

            var order = Order.CreateFrom(
                data,
                pricing,
                orderNumber);

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChanges();

            return order;
        }

       
        #endregion


        #region Getters

        public async Task<OrderForSellerActionDto?> GetOrderForSellerActionAsync(int orderId)
        {
            return await _orderRepository.GetOrderForSellerAction(orderId).FirstOrDefaultAsync();

        }

        public async Task<OrderDetailsDto?> GetOrderByIdAsync(
          int orderId,
          int actorUserId)
        {
            var OrderDetails = await _orderRepository.GetOrderDetails(orderId).FirstOrDefaultAsync();
            if (OrderDetails == null)
                throw new NotFoundException("Order Not Found");

            var hasAccess =
                     OrderDetails.BuyerUserId == actorUserId ||
                     OrderDetails.SellerUserId == actorUserId;

            if (!hasAccess)
             throw new UnauthorizedAccessException("You are not allowed to view this order");

            return OrderDetails;

        }
        public async Task<IEnumerable<DTOs.Order.OrderListSellerDto>> GetOrdersForSellerAsync(int sellerUserId)
        {
            var orders = await _orderRepository.GetOrdersForSeller(sellerUserId).ToListAsync();

            return orders;
        }
        public async Task<IEnumerable<DTOs.Order.OrderListBuyerDto>> GetOrdersForBuyerAsync(int BuyerId)
        {
            var orders = await _orderRepository.GetOrdersForBuyer(BuyerId).ToListAsync();

            return orders;
        }
        #endregion


        #region State transitions

        //// Seller proposes execution (sets ExecutionDays) -> move to BuyerPending
        public async Task<Order> ProposeExecutionAsync(
        Order order,
        int executionDays)
        {
            if (order.Status != OrderStatus.SellerPending)
                throw new InvalidOperationException("Order not in SellerPending state.");

            order.Status=OrderStatus.BuyerPending;
            order.ExecutionDays = executionDays;
            await _orderRepository.SaveChanges();

            return order;
        }
        public async Task<Order> LoadOrderOrThrowAsync(int orderId)
        {
            var shop = await _orderRepository.GetByIDWithTrackingAsync(orderId);

            if (shop == null)
                throw new NotValidException("Shop not found.");

            return shop;
        } 
         public async Task<OrderDto> ConfirmExecutionByBuyerAsync(int orderId, int buyerUserId, bool accept)
         {
             var order = await _orderRepository.GetByIDWithTrackingAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            if (order.UserId != buyerUserId)
                throw new ForbiddenException("You are not the buyer of this order.");

            if (order.Status != OrderStatus.BuyerPending)
                throw new NotValidException("Order is not in BuyerPending state.");

           
            if (!accept)
            {
                order.Status = OrderStatus.SellerPending; 
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.SaveChanges();
                return MapToDto(order);
            }

            order.Status = OrderStatus.InProgress;
            order.ConfirmedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.SaveChanges();
            return MapToDto(order);
         }
       
        //// Seller marks the order as finished and ready for delivery -> CompletedBySeller
        public async Task MarkCompletedBySeller(Order order)
        {
            if (order.Status != OrderStatus.InProgress)
                throw new InvalidOperationException("Order not in progress.");

            order.Status = OrderStatus.CompletedBySeller;
            order.UpdatedAt = DateTime.UtcNow;
           await _orderRepository.SaveChanges();

        }
         

        //// Buyer confirms delivery -> Delivered
        public async Task<OrderDto> ConfirmDeliveryByBuyerAsync(int orderId, int buyerUserId)
        {
            var order = await _orderRepository.GetByIDWithTrackingAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            if (order.UserId != buyerUserId)
                throw new ForbiddenException("Not buyer of this order.");

            if (order.Status != OrderStatus.CompletedBySeller)
                throw new NotValidException("Order not in a state awaiting buyer confirmation (CompletedBySeller).");

            order.Status = OrderStatus.Delivered;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.SaveChanges();

            return MapToDto(order);
        }

        //// Cancel order (either party in allowed states)
        public async Task CancelAsync(Order order)
        {
            if (order.Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel a delivered order.");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.SaveChanges();
        }

        #endregion


        #region Private Functions
        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4]}";
        }
        private PricingResult CalculatePricing(decimal subtotal)
        {
            const decimal shipping = 15m;
            const decimal tax = 0m;

            return new PricingResult
            {
                ShippingFee = shipping,
                TaxTotal = tax,
                GrandTotal = subtotal + shipping + tax
            };
        }
        private static OrderDto MapToDto(Order o)
        {
            return new OrderDto
            {
                OrderId = o.Id,
                CartItemId = o.CartItemId ?? 0,
                ProductId = o.ProductId,
                ProductTitle = o.ProductTitleSnapshot,
                UnitPrice = o.UnitPriceSnapshot,
                Quantity = o.Quantity,
                Subtotal = o.Subtotal,
                ShippingFee = o.ShippingFee,
                GrandTotal = o.GrandTotal,
                ShopId = o.ShopId,
                BuyerId = o.UserId,
                Status = o.Status,
                ExecutionDays = o.ExecutionDays,
                ConfirmedAt = o.ConfirmedAt,
                CreatedAt = o.CreatedAt
            };
        }
        #endregion


    }
}
