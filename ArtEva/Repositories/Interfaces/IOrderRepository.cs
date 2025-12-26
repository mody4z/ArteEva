// IOrderRepository.cs
using ArteEva.Models;
using System.Linq;

namespace ArteEva.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IQueryable<Order> QueryBySeller(int sellerUserId); // seller = owner of shop
        IQueryable<Order> QueryByBuyer(int buyerUserId);

        Task<Order?> GetByIdWithTrackingAsync(int orderId);
        Task<IEnumerable<Order>> GetByIdsAsync(IEnumerable<int> ids);
    }
}
