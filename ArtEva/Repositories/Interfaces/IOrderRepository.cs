// IOrderRepository.cs
using ArteEva.Models;
using ArtEva.DTOs.Order;
using ArtEva.Services.Interfaces;
using System;
using System.Linq;

namespace ArteEva.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
         IQueryable<OrderDetailsDto?> GetOrderDetails(int orderId);
         IQueryable<ArtEva.DTOs.Order.OrderListSellerDto> GetOrdersForSeller(int sellerUserId);
         IQueryable<ArtEva.DTOs.Order.OrderListBuyerDto> GetOrdersForBuyer(int buyerUserId);
        IQueryable<OrderForSellerActionDto> GetOrderForSellerAction(int orderId);



    }
}
