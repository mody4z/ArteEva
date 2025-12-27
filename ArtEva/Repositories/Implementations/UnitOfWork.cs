using ArteEva.Data;
using ArteEva.Repositories;
using ArteEva.Repositories.Implementations;
using ArtEva.Repositories.Interfaces;

namespace ArtEva.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(ApplicationDbContext context) 
        {
          _context = context;
            ShopRepository = new ShopRepository(_context);
            ProductRepository = new ProductRepository(_context);
            ProductImageRepository = new ProductImageRepository(_context);
            CartItemRepository = new CartItemRepository(_context);
            CartRepository = new CartRepository(_context);
            AddressRepository = new AddressRepository(_context);
            CategoryRepository = new CategoryRepository(_context);
            SubCategoryRepository = new SubCategoryRepository(_context);
        }
        private readonly ApplicationDbContext _context;
        public IShopRepository ShopRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IProductImageRepository ProductImageRepository { get; }
        public ICartRepository CartRepository { get; }
        public IAddressRepository AddressRepository { get; }
        public ICartItemRepository CartItemRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public ISubCategoryRepository SubCategoryRepository { get; }

        public async Task<int> SaveChagngesAsync()
        {
           return await _context.SaveChangesAsync();
        }
        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

    }
}
