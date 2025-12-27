using ArteEva.Models;
using ArtEva.DTOs.Order;

namespace ArteEva.Repositories
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        IQueryable<CreateOrderFromCartItemDto> GetOrderInfoForCartItem(int cartItemId);

        IQueryable<CartItem> QueryByCart(int cartId);
        IQueryable<CartItem> QueryByUser(int userId);

        Task<CartItem?> GetByCartAndProductAsync(int cartId, int productId);
        Task<IEnumerable<CartItem>> GetNotConvertedByCartAsync(int cartId);

        Task RemoveRangeAsync(IEnumerable<CartItem> items);
    }
}
