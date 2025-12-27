using ArteEva.Models;
using ArtEva.DTOs.Order;
using ArtEva.DTOs.Orders;
using ArtEva.Models.Enums;
using ArtEva.Services;
using ArtEva.Services.Implementation;
using ArtEva.Services.Interfaces;

public sealed class OrderOrchestrator:IOrderOrchestrator
{
    private readonly ICartItemService _cartItemService;
    private readonly IOrderService _orderService;
    private readonly IShopService _shopService;

    public OrderOrchestrator(
        ICartItemService cartItemService,
        IOrderService orderService,IShopService shopService)
    {
        _cartItemService = cartItemService;
        _orderService = orderService;
        _shopService = shopService;
    }

    public async Task<OrderDto> CreateOrderFromCartItemAsync(int userId,int cartItemId)
    {
        var data = await _cartItemService.GetOrderInfoForCartItemAsync(cartItemId);

        if (data == null)
            throw new NotFoundException ("Cart_item not found");

        if (data.UserId != userId)
            throw new ForbiddenException("Not Allow TO Access ");

        if (data.IsConvertedToOrder)
            throw new NotValidException("Already converted");

        var order = await _orderService.CreateFromCartItemAsync(data);

        await _cartItemService.MarkAsConvertedAsync(
            data.CartItemId,
            order.Id);

        return MapToDto(order);
    }
    public async Task<OrderDto> ProposeExecutionBySellerAsync(int orderId,int sellerUserId,int executionDays)
    {
        var order =
             await _orderService
            .LoadOrderOrThrowAsync(orderId)
            ?? throw new NotFoundException("Order not found");

        if (order.Status != OrderStatus.SellerPending)
            throw new NotValidException("Order not in SellerPending state");

        var shop =await _shopService
            .EnsureSellerCanManageOrdersAsync(
                sellerUserId,
                order.ShopId);
        if (shop.OwnerUserId != sellerUserId)
            throw new ForbiddenException("You are not the seller of this order.");

        var updatedOrder = await _orderService
            .ProposeExecutionAsync(order, executionDays);

        return MapToDto(updatedOrder);
    }
    public async Task<OrderDto> MarkOrderWaitingDeliveryAsync(int orderId, int sellerUserId)
    {
        var order =
              await _orderService
             .LoadOrderOrThrowAsync(orderId)
             ?? throw new NotFoundException("Order not found");

       var shop=  await _shopService
            .EnsureSellerCanManageOrdersAsync(sellerUserId,order.ShopId);
        if (shop.OwnerUserId != sellerUserId)
            throw new ForbiddenException("You are not the seller of this order.");


        await _orderService.MarkCompletedBySeller(order);

         return MapToDto(order);
    }

    public async Task CancelOrderAsync(int orderId, int actorUserId, string reason)
    {
        var order =
               await _orderService
              .LoadOrderOrThrowAsync(orderId)
              ?? throw new NotFoundException("Order not found");

       var shop =await _shopService
           .EnsureSellerCanManageOrdersAsync(actorUserId, order.ShopId);

        var isBuyer = order.UserId == actorUserId;
        var isSeller = shop.OwnerUserId == actorUserId;

        if (!isBuyer && !isSeller)
                 throw new ForbiddenException("You are not the seller or Buyer of this order.");

       await _orderService.CancelAsync(order, reason,actorUserId);

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
}
