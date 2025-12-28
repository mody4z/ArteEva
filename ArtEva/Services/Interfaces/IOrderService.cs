// IOrderService.cs
using ArteEva.Models;
using ArtEva.Application.Orders.Quiries;
using ArtEva.DTOs.Order;
using ArtEva.DTOs.Orders;
using ArtEva.Services.Implementation;
using System;
namespace ArtEva.Services.Interfaces
{
    public interface IOrderService
    {
        public  Task<OrderPagedResult<OrderListSellerDto>> GetOrdersForSellerAsync(
          int sellerUserId,
          OrderQueryCriteria criteria,
          int pageNumber,
          int pageSize);

        public Task<OrderPagedResult<OrderListBuyerDto>> GetOrdersForBuyerAsync(
              int buyerUserId,
              OrderQueryCriteria criteria,
              int pageNumber,
              int pageSize);

         public Task<Order> CreateFromCartItemAsync(CreateOrderFromCartItemDto data);

        //// Getters
        public Task<OrderDetailsDto?> GetOrderByIdAsync(int orderId, int actorUserId);

        //public Task<IEnumerable<DTOs.Order.OrderListSellerDto>> GetOrdersForSellerAsync(int sellerUserId);

        //public   Task<IEnumerable<DTOs.Order.OrderListBuyerDto>> GetOrdersForBuyerAsync(int BuyerId);
        public Task<OrderForSellerActionDto?> GetOrderForSellerActionAsync(int orderId);

        public Task<Order> ProposeExecutionAsync(Order order, int executionDays);
        public  Task<Order> LoadOrderOrThrowAsync(int orderId);
        public Task MarkCompletedBySeller(Order order);
        public Task CancelAsync(Order order, string reason, int actorUserId);
        public Task<OrderDto> ConfirmDeliveryByBuyerAsync(int orderId, int buyerUserId);
        public Task<OrderDto> ConfirmExecutionByBuyerAsync(int orderId, int buyerUserId, bool accept);


    }
}
