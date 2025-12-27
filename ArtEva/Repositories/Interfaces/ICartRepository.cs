using ArteEva.Models;

namespace ArteEva.Repositories
{
    public interface ICartRepository : IRepository<Cart>
    {
        /// <summary>
        /// Gets queryable for carts belonging to a user.
        /// </summary>
        IQueryable<Cart> GetCartsByUserQuery(int userId);

        /// <summary>
        /// Gets or creates a tracked cart for the user.
        /// </summary>
        Task<Cart> GetOrCreateTrackedCartAsync(int userId);

        /// <summary>
        /// Gets tracked cart by ID.
        /// </summary>
        Task<Cart?> GetTrackedCartByIdAsync(int cartId);
    }
}
