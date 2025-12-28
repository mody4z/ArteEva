// OrderRepository.cs
using ArteEva.Data;
using ArteEva.Models;
using ArtEva.Application.Products.Specifications;
using ArtEva.DTOs.Order;
using ArtEva.Models.Enums;
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
        public IQueryable<OrderDetailsDto> GetOrderDetails(int orderId)
        {
            return GetAllAsync()
                .Where(o => o.Id == orderId)
                .Select(o => new OrderDetailsDto
                {
                    OrderId = o.Id,
                    BuyerUserId = o.UserId,
                    SellerUserId = o.Shop.OwnerUserId,

                    OrderNumber = o.OrderNumber,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    GrandTotal = o.GrandTotal,

                    Quantity = o.Quantity,
                    UnitPrice = o.UnitPriceSnapshot,
                    Subtotal = o.Subtotal,
                    ShippingFee = o.ShippingFee,
                    TaxTotal = o.TaxTotal,

                    ExecutionDays = o.ExecutionDays,
                    ConfirmedAt = o.ConfirmedAt,
                    ExpectedDeliveryDate = o.ExpectedDeliveryDate,

                    ProductTitle = o.ProductTitleSnapshot,
                    ProductImage = o.ProductImageSnapshot,

                    Cancellation = o.Status == OrderStatus.Cancelled
                        ? new CancellationInfoDto
                        {
                            Reason = o.CancellationReason,
                            CancelledByUserId = o.CancelledByUserId!.Value,
                            CancelledAt = o.CancelledAt!.Value
                        }
                        : null
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

        //public async Task<IReadOnlyList<Order>> GetPagedAsync(
        //ISpecification<Order> specification,
        //int pageNumber,
        //int pageSize)
        //{
        //    var query = _context.Orders.Where(specification.Criteria);

        //    if (specification.OrderBy != null)
        //        query = query.OrderBy(specification.OrderBy);

        //    if (specification.OrderByDescending != null)
        //        query = query.OrderByDescending(specification.OrderByDescending);

        //    return await query
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();
        //}

        public IQueryable<Order> GetPagedQuery(
             ISpecification<Order> specification)
        {
            var query = _context.Orders
                .Where(specification.Criteria);

            if (specification.OrderBy != null)
                query = query.OrderBy(specification.OrderBy);

            if (specification.OrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending);

            return query;
        }

        public async Task<int> CountAsync(ISpecification<Order> specification)
        {
            return await _context.Orders
                .Where(specification.Criteria)
                .CountAsync();
        }



    }
}
