using ArtEva.DTOs.Order;
using ArtEva.DTOs.Orders;

namespace ArtEva.Services.Interfaces
{
    public interface IOrderOrchestrator
    {
        public Task<OrderDto> CreateOrderFromCartItemAsync(int userId, int cartItemId);
        public Task<OrderDto> ProposeExecutionBySellerAsync( int orderId,int sellerUserId,int executionDays);
        public Task CancelOrderAsync(int orderId, int actorUserId, string reason);
        public Task<OrderDto> MarkOrderWaitingDeliveryAsync(int orderId, int sellerUserId);

    }
}
