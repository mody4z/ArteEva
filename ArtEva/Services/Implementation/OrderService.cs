// OrderService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArteEva.Data;
using ArteEva.Models;
using ArteEva.Repositories;
using ArtEva.DTOs.Orders;
using ArtEva.Models.Enums;
using ArtEva.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArtEva.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IShopRepository _shopRepository;
        private readonly ApplicationDbContext _dbContext;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IShopRepository shopRepository,
            ApplicationDbContext dbContext)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _shopRepository = shopRepository;
            _dbContext = dbContext;
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4]}";
        }


        //// ----------------- Checkout: convert cart items to orders -----------------
        //public async Task<IEnumerable<OrderDto>> CreateOrdersFromCartAsync(int userId)
        //{
        //    var cart = await _cartRepository.GetOrCreateCartWithTrackingAsync(userId);
        //    if (cart == null) throw new InvalidOperationException("Cart not found");

        //    // get all not-converted items for this cart
        //    var items = (await _cartItemRepository.GetNotConvertedByCartAsync(cart.Id)).ToList();
        //    if (items == null || !items.Any()) return new List<OrderDto>();

        //    var createdOrders = new List<Order>();

        //    using var tx = await _dbContext.Database.BeginTransactionAsync();
        //    try
        //    {
        //        // 1) Create Order entities for each cart item (do not assume saved Id yet)
        //        foreach (var ci in items)
        //        {
        //            if (ci.IsConvertedToOrder) continue; // concurrent safety

        //            var order = new Order
        //            {
        //                UserId = cart.UserId,
        //                ShopId = (ci.Product != null && ci.Product.ShopId != 0) ? ci.Product.ShopId : ci.Id,
        //                ProductId = ci.ProductId,
        //                Quantity = ci.Quantity,
        //                UnitPriceSnapshot = ci.UnitPrice,
        //                ProductTitleSnapshot = ci.ProductName,
        //                Subtotal = ci.TotalPrice,
        //                ShippingFee = 15m,
        //                TaxTotal = 0m,
        //                GrandTotal = ci.TotalPrice + 15m,
        //                Status = OrderStatus.SellerPending,
        //                CreatedAt = DateTime.UtcNow,
        //                ConfirmedAt = null,
        //                ExecutionDays = null,
        //                CartItemId = ci.Id ,   // link back (if Order entity has this property)
        //                 OrderNumber = GenerateOrderNumber()
        //            };

        //            await _orderRepository.AddAsync(order);
        //            createdOrders.Add(order);
        //        }

        //        // 2) persist orders (so they get IDs)
        //        await _orderRepository.SaveChanges();

        //        // 3) mark cart items as converted and set OrderId for traceability
        //        //    Pairing by CartItemId (safer than relying on ordering)
        //        // Build mapping from createdOrders' CartItemId -> order
        //        var orderByCartItem = createdOrders
        //            .Where(o => o.CartItemId != 0)
        //            .ToDictionary(o => o.CartItemId, o => o);

        //        foreach (var ci in items)
        //        {
        //            if (ci.IsConvertedToOrder) continue;

        //            // try to find created order for this cart item
        //            if (orderByCartItem.TryGetValue(ci.Id, out var ord))
        //            {
        //                ci.IsConvertedToOrder = true;
        //                ci.OrderId = ord.Id;
        //            }
        //            else
        //            {
        //                // fallback: if no mapping (shouldn't happen), try match by characteristics
        //                var fallback = createdOrders.FirstOrDefault(o =>
        //                    o.ProductId == ci.ProductId &&
        //                    o.Subtotal == ci.TotalPrice &&
        //                    o.ShopId == ((ci.Product != null && ci.Product.ShopId != 0) ? ci.Product.ShopId : ci.Id) &&
        //                    !o.IsDeleted);

        //                if (fallback != null)
        //                {
        //                    ci.IsConvertedToOrder = true;
        //                    ci.OrderId = fallback.Id;
        //                }
        //            }
        //        }

        //        await _cartItemRepository.SaveChanges();

        //        await tx.CommitAsync();
        //    }
        //    catch
        //    {
        //        await tx.RollbackAsync();
        //        throw;
        //    }

        //    // map and return DTOs
        //    var result = createdOrders.Select(o => MapToDto(o)).ToList();
        //    return result;
        //}

        // ----------------- Getters -----------------

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int actorUserId)
        {
            var order = await _orderRepository.GetByIdWithTrackingAsync(orderId);
            if (order == null) return null;

            // optional: ensure actor authorization (buyer or seller)
            // var shop = await _shopRepository.GetByIdAsync(order.ShopId);
            // if (!(order.UserId == actorUserId || shop.OwnerUserId == actorUserId)) throw new UnauthorizedAccessException();

            return MapToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersForSellerAsync(int sellerUserId)
        {
            var orders = await _orderRepository.QueryBySeller(sellerUserId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersForBuyerAsync(int buyerUserId)
        {
            var orders = await _orderRepository.QueryByBuyer(buyerUserId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByIdsAsync(IEnumerable<int> ids)
        {
            var orders = await _orderRepository.GetByIdsAsync(ids);
            return orders.Select(MapToDto);
        }

        // ----------------- State transitions -----------------

        // Seller proposes execution (sets ExecutionDays) -> move to BuyerPending
        public async Task<OrderDto> ProposeExecutionBySellerAsync(int orderId, int sellerUserId, int executionDays)
        {
            var order = await _orderRepository.GetByIdWithTrackingAsync(orderId)
                ?? throw new InvalidOperationException("Order not found");

            // verify seller owns the shop
            var shop = await _shopRepository.GetByIdAsync(order.ShopId)
                ?? throw new InvalidOperationException("Shop not found");

            if (shop.OwnerUserId != sellerUserId)
                throw new UnauthorizedAccessException("You are not the seller of this order.");

            if (order.Status != OrderStatus.SellerPending)
                throw new InvalidOperationException("Order not in SellerPending state.");

            order.ExecutionDays = executionDays;
            order.Status = OrderStatus.BuyerPending;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.SaveChanges();

            return MapToDto(order);
        }

        // Buyer confirms the proposed schedule -> move to InProgress
        public async Task<OrderDto> ConfirmExecutionByBuyerAsync(int orderId, int buyerUserId, bool accept)
        {
            var order = await _orderRepository.GetByIdWithTrackingAsync(orderId)
                ?? throw new InvalidOperationException("Order not found");

            if (order.UserId != buyerUserId)
                throw new UnauthorizedAccessException("Not buyer of this order.");

            if (order.Status != OrderStatus.BuyerPending)
                throw new InvalidOperationException("Order not in BuyerPending state.");

            if (!accept)
            {
                // revert to seller pending to renegotiate or cancel
                order.Status = OrderStatus.SellerPending;
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.SaveChanges();
                return MapToDto(order);
            }

            // accept -> go InProgress
            order.Status = OrderStatus.InProgress;
            order.ConfirmedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.SaveChanges();
            return MapToDto(order);
        }

        // Seller marks the order as finished and ready for delivery -> CompletedBySeller
        public async Task<OrderDto> MarkOrderWaitingDeliveryAsync(int orderId, int sellerUserId)
        {
            var order = await _orderRepository.GetByIdWithTrackingAsync(orderId)
                ?? throw new InvalidOperationException("Order not found");

            var shop = await _shop_repository_gettracking(order.ShopId);

            if (shop.OwnerUserId != sellerUserId)
                throw new UnauthorizedAccessException("You are not the seller of this order.");

            if (order.Status != OrderStatus.InProgress)
                throw new InvalidOperationException("Order not in progress.");

            order.Status = OrderStatus.CompletedBySeller;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.SaveChanges();

            return MapToDto(order);
        }

        private async Task<Shop> _shop_repository_gettracking(int shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null) throw new InvalidOperationException("Shop not found");
            return shop;
        }

        // Buyer confirms delivery -> Delivered
        public async Task<OrderDto> ConfirmDeliveryByBuyerAsync(int orderId, int buyerUserId)
        {
            var order = await _orderRepository.GetByIdWithTrackingAsync(orderId)
                ?? throw new InvalidOperationException("Order not found");

            if (order.UserId != buyerUserId)
                throw new UnauthorizedAccessException("Not buyer of this order.");

            // buyer can only confirm delivery after seller finished work
            if (order.Status != OrderStatus.CompletedBySeller)
                throw new InvalidOperationException("Order not in a state awaiting buyer confirmation (CompletedBySeller).");

            order.Status = OrderStatus.Delivered;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.SaveChanges();

            return MapToDto(order);
        }

        // optional: seller starts order (set InProgress) — sometimes buyer already confirmed
        public async Task<OrderDto> MarkOrderInProgressAsync(int orderId, int sellerUserId)
        {
            var order = await _orderRepository.GetByIdWithTrackingAsync(orderId)
                ?? throw new InvalidOperationException("Order not found");

            var shop = await _shopRepository.GetByIdAsync(order.ShopId)
                ?? throw new InvalidOperationException("Shop not found");

            if (shop.OwnerUserId != sellerUserId)
                throw new UnauthorizedAccessException("You are not the seller of this order.");

            if (order.Status != OrderStatus.BuyerPending)
                throw new InvalidOperationException("Order not in BuyerPending (cannot start).");

            order.Status = OrderStatus.InProgress;
            order.ConfirmedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.SaveChanges();

            return MapToDto(order);
        }

        // Cancel order (either party in allowed states)
        public async Task CancelOrderAsync(int orderId, int actorUserId, string reason)
        {
            var order = await _orderRepository.GetByIdWithTrackingAsync(orderId)
                ?? throw new InvalidOperationException("Order not found");

            // simple rule: buyer or shop owner can cancel only if not delivered
            var shop = await _shopRepository.GetByIdAsync(order.ShopId)
                ?? throw new InvalidOperationException("Shop not found");

            var isBuyer = order.UserId == actorUserId;
            var isSeller = shop.OwnerUserId == actorUserId;

            if (!isBuyer && !isSeller)
                throw new UnauthorizedAccessException("Not authorized to cancel this order.");

            if (order.Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel a delivered order.");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            // optionally store cancellation reason in a new field

            await _orderRepository.SaveChanges();
        }

        // ----------------- Helpers -----------------

        private static OrderDto MapToDto(Order o)
        {
            return new OrderDto
            {
                OrderId = o.Id,
                CartItemId = o.CartItemId??0,
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

        public Task<IEnumerable<OrderDto>> CreateOrdersFromCartAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
