// IOrderService.cs
using ArtEva.DTOs.Orders;
namespace ArtEva.Services.Interfaces
{
    public interface IOrderService
    {
        // Checkout: convert all non-converted cart items for the user into Orders (one order per CartItem)
        Task<IEnumerable<OrderDto>> CreateOrdersFromCartAsync(int userId);

        // Getters
        Task<OrderDto?> GetOrderByIdAsync(int orderId, int actorUserId);
        Task<IEnumerable<OrderDto>> GetOrdersForSellerAsync(int sellerUserId);
        Task<IEnumerable<OrderDto>> GetOrdersForBuyerAsync(int buyerUserId);

        // State transitions (Seller sets schedule -> Buyer approves -> Execution -> Delivery -> Complete)
        Task<OrderDto> ProposeExecutionBySellerAsync(int orderId, int sellerUserId, int executionDays);
        Task<OrderDto> ConfirmExecutionByBuyerAsync(int orderId, int buyerUserId, bool accept);
        Task<OrderDto> ConfirmDeliveryByBuyerAsync(int orderId, int buyerUserId);
        Task CancelOrderAsync(int orderId, int actorUserId, string reason);
        Task<OrderDto> MarkOrderWaitingDeliveryAsync(int orderId, int sellerUserId);

        // Helpers
        Task<IEnumerable<OrderDto>> GetOrdersByIdsAsync(IEnumerable<int> ids);
        Task<OrderDto> MarkOrderInProgressAsync(int orderId, int sellerUserId); // optional
    }
}
