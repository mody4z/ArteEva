using ArteEva.Models;

namespace ArteEva.Repositories
{
    public interface ICartRepository : IRepository<Cart>
    {
        IQueryable<Cart> QueryByUser(int userId);

        Task<Cart?> GetOrCreateCartWithTrackingAsync(int userId);
        Task<Cart?> GetByIdWithTrackingAsync(int cartId);
    }
}
