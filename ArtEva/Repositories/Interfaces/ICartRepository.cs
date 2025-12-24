using ArteEva.Models;

namespace ArteEva.Repositories
{
    public interface ICartRepository : IRepository<Cart>
    {
        public   Task<Cart?> GetOrCreateCartAsync(int userId);

    }
}
