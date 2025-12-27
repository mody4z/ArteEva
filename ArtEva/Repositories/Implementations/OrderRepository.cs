// OrderRepository.cs
using ArteEva.Data;
using ArteEva.Models;
using ArtEva.DTOs.Order;
using ArtEva.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ArteEva.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public  IQueryable<OrderDetailsDto?>  GetOrderDetails(int orderId)
        {
            return   
                GetAllAsync()
                .Where(o => o.Id == orderId)
                .Select(o => new OrderDetailsDto
                {
                    OrderId = o.Id,
                    BuyerUserId = o.UserId,
                    SellerUserId = o.Shop.OwnerUserId,

                    OrderNumber = o.OrderNumber,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    GrandTotal = o.GrandTotal
                });
                
        }
        public   IQueryable<ArtEva.DTOs.Order.OrderListSellerDto> GetOrdersForSeller(int sellerUserId)
        {
            return
                 GetAllAsync()
                .Where(o => o.Shop.OwnerUserId == sellerUserId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new ArtEva.DTOs.Order.OrderListSellerDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    GrandTotal = o.GrandTotal,
                    ExecutionDays = o.ExecutionDays
                });
               
        }
        public IQueryable<OrderForSellerActionDto> GetOrderForSellerAction(int orderId)
        {
            return GetAllAsync()
                .Where(o => o.Id == orderId)
                .AsTracking()
                .Select(o => new OrderForSellerActionDto
                {
                    OrderId = o.Id,
                    ShopId = o.ShopId,
                    Status = o.Status,
                    ExecutionDays=o.ExecutionDays,
                    GrandTotal = o.GrandTotal,
                   BuyerUserId=o.UserId
                });
        }
        public IQueryable<ArtEva.DTOs.Order.OrderListBuyerDto> GetOrdersForBuyer(int buyerUserId)
        {
            return
                 GetAllAsync()
                .Where(o =>o.UserId == buyerUserId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new ArtEva.DTOs.Order.OrderListBuyerDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    GrandTotal = o.GrandTotal
                });
        }

    }
}
