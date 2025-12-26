// OrderRepository.cs
using ArteEva.Data;
using ArteEva.Models;
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

        public IQueryable<Order> QueryBySeller(int sellerUserId)
        {
            // Assumes Shop.OwnerUserId holds seller's user id
            return _context.Orders
                .AsNoTracking()
                .Where(o => !o.IsDeleted && o.Shop != null && o.Shop.OwnerUserId == sellerUserId);
        }

        public IQueryable<Order> QueryByBuyer(int buyerUserId)
        {
            return _context.Orders
                .AsNoTracking()
                .Where(o => !o.IsDeleted && o.UserId == buyerUserId);
        }

        public async Task<Order?> GetByIdWithTrackingAsync(int orderId)
        {
            return await _context.Orders
                .AsTracking()
                .Include(o => o.Shop) // include shop so we can check ownership
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
        }

        public async Task<IEnumerable<Order>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Orders
                .Where(o => ids.Contains(o.Id) && !o.IsDeleted)
                .ToListAsync();
        }
    }
}
